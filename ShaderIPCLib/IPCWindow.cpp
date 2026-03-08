#include "IPCWindow.h"

#include <stdexcept>

static ATOM    registered_class = 0;
static LPCWSTR className           = L"IPCWindow";

IPCWindow::~IPCWindow()
{
    ClearLifecycleEvents();
    SetStatedataFromMemBlock<PartnerCriteria>(nullptr);
    partner = nullptr;
}
void IPCWindow::ForceFailState()
{
    if (state >= IPCWCode::NewlyCreated)
    {
        auto h = self;
        self   = nullptr;
        if(h)
            DestroyWindow(h);
    }
    state = IPCWCode::IPCError;
    
}
void* IPCWindow::GetStatedataPtr()
{
    return statedata;
}

template<typename Type>
Type IPCWindow::GetStatedata()
{
    return *((Type*)statedata);
}

template <typename Type>
bool IPCWindow::SetStatedataFromMemBlock(Type* data)
{
    void* statedata = this->statedata;
    if(statedata)
    {
        delete statedata;
        this->statedata = nullptr;
    }
    if(!data)
        return true;

    statedata = new Type();
    if(!statedata)
        return false;
    memcpy(statedata, data, sizeof(Type));
    this->statedata = statedata;
    return true;
}

void IPCWindow::SetPartnerDetails(int partner_pid , LPCWSTR partner_name, LPCWSTR partner_class )
    {
        if(state != None && state != NewlyCreated) throw "SetPartnerDetails must be called before handshake functions";
        PartnerCriteria advert(partner_pid, partner_name, partner_class);
        SetStatedataFromMemBlock(&advert);
    }

ATOM IPCWindow::CustomRegisterClass(HINSTANCE hInstance, LPCWSTR class_name)
    {
        WNDCLASS wc      = {0};
        wc.lpfnWndProc   = IPCWindowProc;
        wc.hInstance     = hInstance;
        wc.lpszClassName = class_name;
        wc.style         = 0;
        return registered_class = RegisterClass(&wc);
    }

HWND IPCWindow::Create(HINSTANCE hInstance, LPCWSTR name)
    {
        if(!registered_class)
            CustomRegisterClass(hInstance, className);
        return CreateWindowW(MAKEINTATOM(registered_class), name ? name : className, 0, 0, 0, 100, 100, HWND_MESSAGE, nullptr, hInstance, this);
    }
    
LRESULT IPCWindow::IPCWindowProc(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam)
{
    IPCWindow* app;
    if(msg == WM_CREATE)
    {
        app = (IPCWindow*)(((LPCREATESTRUCT)lParam)->lpCreateParams);
        SetWindowLongPtr(hWnd, GWLP_USERDATA, (LONG_PTR)app);
        app->state = NewlyCreated;
        app->self  = hWnd;
    }
    else
    {
        app = (IPCWindow*)GetWindowLongPtr(hWnd, GWLP_USERDATA);
        if(!app)
            return DefWindowProc(hWnd, msg, wParam, lParam);
    }
    return app->WindowProc(hWnd, msg, wParam, lParam);
}

#include "tlhelp32.h"

PartnerCriteria IPCWindow::determine_partner_criteria() {
    PartnerCriteria advert;
    void*           stored = GetStatedataPtr();
    if(stored == nullptr)
        advert = PartnerCriteria();
    else
        advert = *(PartnerCriteria*)stored;
    int pid = std::get<0>(advert);
    if(pid == 0)
    {
        HANDLE hSnapShot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
        if (hSnapShot != INVALID_HANDLE_VALUE)
        {
            pid = GetProcessId(GetCurrentProcess());
            PROCESSENTRY32 procentry;
            memset((LPVOID)&procentry, 0, sizeof(PROCESSENTRY32));
            procentry.dwSize = sizeof(PROCESSENTRY32);
            for(bool bContinue = Process32First(hSnapShot, &procentry); bContinue; bContinue = Process32Next(hSnapShot, &procentry))
            {
                if(pid == procentry.th32ProcessID)
                {
                    pid = procentry.th32ParentProcessID;
                    break;
                }
                procentry.dwSize = sizeof(PROCESSENTRY32);
            }
        }
    }

    LPCWSTR s;
    advert = PartnerCriteria(pid, (s = std::get<1>(advert)) ? s : className, (s = std::get<2>(advert)) ? s : className);
    SetStatedataFromMemBlock(&advert);
    return advert;
}

void IPCWindow::AnticipateHandshake() 
{
    switch (state)
    {
    case None:
        throw "Cannot participate in handshake before window is created; you can use the AfterCreated(IPCWindow) callback for this.";
    case NewlyCreated:
        state = AnticipatingHandshake;
        return;
    }
    throw "Invalid state for AnticipateHandshake()";
}
bool IPCWindow::CheckWindowPid(HWND window, int pid)
{
    DWORD wpid;
    return GetWindowThreadProcessId(window, &wpid) && wpid == pid;
}

bool IPCWindow::CheckWindowStrings(HWND window, LPCWSTR className, LPCWSTR windowName) {
    if(className)
    {
        auto len = wcslen(className);
        if(len >= UINT32_MAX)
            return false;
        wchar_t* cls = new wchar_t[len + 1];
        if(RealGetWindowClass(window, cls, len + 1) != len || wcscmp(className, cls))
        {
            delete[] cls;
            return false;
        }
        delete[] cls;
    }
    if(windowName)
    {
        auto len = wcslen(windowName);
        if(len >= UINT32_MAX)
            return false;
        wchar_t* win = new wchar_t[len + 1];
        if(GetWindowText(window, win, len + 1) != len || wcscmp(windowName, win))
        {
            delete[] win;
            return false;
        }
        delete[] win;
    }
    return true;
}
unsigned long long IPCWindow::SendStringPtr(UINT_PTR msgType, LPCWSTR data)
{
    return SendStringPtr(msgType, data, partner);
}
unsigned long long IPCWindow::SendStringPtr(UINT_PTR msgType, LPCWSTR data, HWND partner)
{
    COPYDATASTRUCT send;
    if(data)
    {
        auto l = wcslen(data);
        if(l > MAXDWORD)
            return FALSE;
        send.cbData = (l + 1) * sizeof(WCHAR);
    }
    else
        send.cbData = 0;
    send.dwData               = msgType;
    send.lpData               = (void*)data;
    unsigned long long result = IPCWCode::Unspecified;
    result = SendMessageTimeout(partner, WM_COPYDATA, (WPARAM)self, (LPARAM)(LPVOID)&send, SMTO_NORMAL, IPCTimeoutMS, &result) ? 
        (result ? result : IPCWCode::Unspecified) : FALSE;
    if(!result)
        if(OnSendFailed)
            OnSendFailed(msgType, this);
    return result;
}

template<typename Type>
unsigned long long IPCWindow::SendData(UINT_PTR msgType, Type data, HWND partner)
{
    return SendDataPtr(msgType, &data, sizeof(Type), partner);
}
unsigned long long IPCWindow::SendDataPtr(UINT_PTR msgType, void* data, size_t size_override)
{
    return SendDataPtr(msgType, data, size_override, partner);
}
unsigned long long IPCWindow::SendDataPtr(UINT_PTR msgType, void* data, size_t size_override, HWND partner)
{
    COPYDATASTRUCT send;
    send.cbData               = size_override;
    send.dwData               = msgType;
    send.lpData               = data;
    unsigned long long result = IPCWCode::Unspecified;
    result = SendMessageTimeout(partner, WM_COPYDATA, (WPARAM)self, (LPARAM)(LPVOID)&send, SMTO_NORMAL, IPCTimeoutMS, &result) ? (result ? result : IPCWCode::Unspecified) : FALSE;
    if(!result)
        if(OnSendFailed)
            OnSendFailed(msgType, this);
    return result;
}
unsigned long long IPCWindow::SendSignal(UINT signal, HWND partner)
{
    unsigned long long result = IPCWCode::Unspecified;
    result = SendMessageTimeout(partner, signal, (WPARAM)self, 0, SMTO_NORMAL, IPCTimeoutMS, &result) ? 
        (result ? result : IPCWCode::Unspecified) : FALSE;
    if(!result)
        if(OnSendFailed)
            OnSendFailed(signal, this);
    return result;
}
template <typename Type>
unsigned long long IPCWindow::SendData(UINT_PTR msgType, Type data)
{
    return SendData(msgType, data, partner);
}
unsigned long long IPCWindow::SendSignal(UINT signal)
{
    return SendSignal(signal, partner);
}
void IPCWindow::PostSignal(UINT signal)
{
    PostMessage(partner, signal, (WPARAM)self, 0);
}

BOOL IPCWindow::QueueReceivedData(HWND from, COPYDATASTRUCT* data)
{
    auto receive    = new COPYDATASTRUCT();
    receive->dwData = data->dwData;
    auto size      = data->cbData;
    receive->cbData = size;
    receive->lpData = malloc(size);
    memcpy_s(receive->lpData, size, data->lpData, size);
    auto addInbound = new std::tuple<HWND, COPYDATASTRUCT*, void**>(from, receive, new void*[1] { nullptr });
    auto q = this->InboundMessageQueue;
    if(q)
    {
        for(;;)
        {
            auto n = std::get<2>(*q)[0];
            if(n)
                q = static_cast<decltype(q)>(n);
            else
                break;
        }
        std::get<2>(*q)[0] = addInbound;
    }
    else
    {
        this->InboundMessageQueue = addInbound;
    }
    BOOL r = PostMessage(self, IPCWCode::NewData, (WPARAM)self, 0);
    if(!r)
    {
        free(receive->lpData);
        delete(receive);
        delete(addInbound);
        if(q)
            std::get<2>(*q)[0] = nullptr;
        else
            this->InboundMessageQueue = nullptr;
    }
    return r;
}

bool IPCWindow::InitiateHandshake()
{
    PartnerCriteria criteria;
    HWND            candidate;
    IPCWCode    outcome = NoMatch;

    #define CHECK_WINPID (std::get<0>(criteria) < 0 || CheckWindowPid(candidate, std::get<0>(criteria)))
    #define CHECK_WINDOW (CHECK_WINPID && CheckWindowStrings(candidate, std::get<2>(criteria), std::get<1>(criteria)))

    auto initiate_ifTrueTryMore = [&](HWND candidate) 
    {
        if(CHECK_WINDOW)
        {
            switch((IPCWCode)SendSignal((ULONG_PTR)InitiatingHandshake, candidate))
            {
            case CompletingHandshake:
                partner = candidate;
                state = CompletingHandshake;
                outcome = CompletingHandshake;
                return FALSE;
            case PresentlyUnavailable:
                partner = candidate;
                outcome = PresentlyUnavailable;
                break;
            case PermanentlyUnavailable:
                if(outcome == NoMatch)
                    outcome = PermanentlyUnavailable;
                break;
            case NoMatch:
                break;
            default:
                if(outcome != PresentlyUnavailable)
                    outcome = PartnerMayHaveFailed;
                break;
            }
        }
        return TRUE;
    };

    switch(state)
    {
    case None:
        throw "Cannot participate in handshake before window is created; you can use the AfterCreated(IPCWindow) callback for this.";
    case NewlyCreated:
        criteria = determine_partner_criteria();
        break;
    case NoMatch:
    case PermanentlyUnavailable:
    case PartnerMayHaveFailed:
        criteria = GetStatedata<PartnerCriteria>();
        break;
    case PresentlyUnavailable:
        candidate = partner;
        goto retry;
    case InitiatingHandshake: 
        // already in the middle of it
        return false;
    case CompletingHandshake:
        return true;
    default:
        if(IsConfirmedPartner(partner))
            return true;
        throw "Invalid state for InitiateHandshake()";
    }

    candidate = FindWindowEx(HWND_MESSAGE, nullptr, std::get<2>(criteria), std::get<1>(criteria));
    if(!candidate)
    {
        state = NoMatch;
        return false;
    }
    if(candidate == self)
        goto EnumerateCandidates;

    state = InitiatingHandshake;
    if(!CHECK_WINPID)
        goto EnumerateCandidates;
    goto connect;

retry:
    state = InitiatingHandshake;
    if(!CHECK_WINDOW)
        goto EnumerateCandidates;

    connect:
    if(!initiate_ifTrueTryMore(candidate))
        return true;

    EnumerateCandidates:
    {
        HWND scope = GetAncestor(self, GA_PARENT);
        state = InitiatingHandshake;
        EnumChildWindows(scope, [](HWND candidate, LPARAM arg) { 
            return (*static_cast<decltype(initiate_ifTrueTryMore)*>((void*)arg))(candidate); }, (LPARAM)&initiate_ifTrueTryMore);
        if(outcome == CompletingHandshake)
            return true;
        state = outcome;
        return false;
    }
}

void IPCWindow::BeginShakingHandsUntilSuccessful(UINT interval_ms)
{
    if(!InitiateHandshake())
        SetTimer(self, IPCWCode::Timer_RepeatUntilComplete, interval_ms, nullptr);
}

void IPCWindow::RequestShowWindowCall(HWND targetInThisProcess, int cmdShow) 
{
    if(IsConfirmedPartner(partner))
    {
        
        auto data = std::tuple<HWND, int>(targetInThisProcess, cmdShow);
        SendData(IPCWCode::CallShowWindowForMe, data);
    }
}


LRESULT IPCWindow::WindowProc(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam)
{
    switch(msg)
    {
    case IPCWCode::NewlyCreated: {
        TriggerLifecycleEvents();
        return TRUE;
    }
    case IPCWCode::NewData: {
        auto dequeue        = InboundMessageQueue;
        auto src            = std::get<0>(*dequeue);
        auto data           = std::get<1>(*dequeue);
        InboundMessageQueue = static_cast<decltype(dequeue)>(*std::get<2>(*dequeue));
        HandleQueuedData(src, data);
        free(data->lpData);
        delete data;
        delete dequeue;
        return 0;
    }
    case IPCWCode::CompletingHandshakeResponse: {
        if(state == CompletingHandshakeResponse)
        {
            SetHandshakeCompletedState();
            SendSignal(state);
            TriggerLifecycleEvents();
        } 
        return 0;
    }
    case IPCWCode::HandshakeComplete: {
        if(state == CompletingHandshake && (HWND)wParam == partner)
        {
            SetHandshakeCompletedState();
            TriggerLifecycleEvents();
        }
        return 0;
    }
    case IPCWCode::IPCError: {
        if(state == CompletingHandshake && (HWND)wParam == partner)
        {
            state = IPCError;
            TriggerLifecycleEvents();
        }
        return 0;
    }

    case IPCWCode::InitiatingHandshake: {
        PartnerCriteria criteria;
        switch(state)
        {
        case AnticipatingHandshake:
            criteria = determine_partner_criteria();
            break;
        case AnticipatingAnotherHandshake:
            criteria = GetStatedata<PartnerCriteria>();
            break;
        case None:
        case NewlyCreated:
            return IPCWCode::PresentlyUnavailable;
        case CompletingHandshake:
            // may choose to test if NoMatch should be sent instead
            return IPCWCode::PresentlyUnavailable;
        case IPCError:
            // may choose to test if NoMatch should be sent instead
            return IPCWCode::PartnerMayHaveFailed;
        default:
            // may choose to test if NoMatch should be sent instead
            return IPCWCode::PermanentlyUnavailable;
        }
        HWND candidate = (HWND)wParam;
        if(CHECK_WINDOW)
        {
            partner = candidate;
            state   = CompletingHandshakeResponse;
            PostMessage(self, CompletingHandshakeResponse, 0, 0);
            return CompletingHandshake;
        }
        return NoMatch;
    }
    case WM_TIMER:
        switch (wParam)
        {
        case IPCWCode::Timer_RepeatUntilComplete:
            switch(state)
            {
            case NoMatch:
            case PermanentlyUnavailable:
            case PartnerMayHaveFailed:
            case PresentlyUnavailable:
                if(!InitiateHandshake())
                    return 0;
                break;
            }
            KillTimer(self, IPCWCode::Timer_RepeatUntilComplete);
            return 0;
        case IPCWCode::Timer_RunOnceWhenEverySubmissionIsReady:
            KillTimer(hWnd, IPCWCode::Timer_RunOnceWhenEverySubmissionIsReady);
            TriggerLifecycleEvents();
            return 0;
        }
        break;

    case WM_CREATE:
        PostMessage(self, IPCWCode::NewlyCreated, 0, 0);
        break;

    case WM_COPYDATA:
        if(IsConfirmedPartner((HWND)wParam))
        {
            auto data = (COPYDATASTRUCT*)lParam;
            if(data->dwData == IPCWCode::ResolveStringToNonZeroUINT_ID && ResolveNameToNonZeroId)
                return ResolveNameToNonZeroId((LPCWSTR)data->lpData, this);
            return HandleReceivedData(partner, data);
        }
        break;

        case WM_DESTROY:
        if(IsConfirmedPartner(partner))
            SendSignal(IPCWCode::PartnerShuttingDown);
        self = NULL;
        break;

        case IPCWCode::PartnerShuttingDown:
        if(IsConfirmedPartner((HWND)wParam))
                if(OnPartnerShuttingDown)
                {
                    state = Unspecified;
                    return OnPartnerShuttingDown(this);
                }
            else
                return HandleReceivedSignal(partner, msg);
        return 0;
            
    default:
        if(msg > (unsigned int)IPCWCode::ExtendedCodeStart && msg <= 0x7FFF && IsConfirmedPartner((HWND)wParam))
                return HandleReceivedSignal(partner, msg);
        break;
    }
    return DefWindowProc(hWnd, msg, wParam, lParam);
}

unsigned int IPCWindow::RequestRemoteId(LPCWSTR name) 
{
    auto id = SendStringPtr(IPCWCode::ResolveStringToNonZeroUINT_ID, name);
    return id > MAXUINT ? 0 : id;
}

void IPCWindow::HandleQueuedData(HWND from, COPYDATASTRUCT* data) {
    switch (data->dwData)
    {
    case IPCWCode::CallShowWindowForMe:
    {
        DWORD pid;
        GetWindowThreadProcessId(partner, &pid);

        auto args = (std::tuple<HWND, int>*)data->lpData;
        from      = std::get<0>(*args);
        
        DWORD wpid;
        if(GetWindowThreadProcessId(from, &wpid) && wpid == pid)
            ShowWindowAsync(from, std::get<1>(*args));
    }
    }
}

LRESULT IPCWindow::HandleReceivedData(HWND from, COPYDATASTRUCT* data)
{ 
    return QueueReceivedData(from, data);
}
LRESULT IPCWindow::HandleReceivedSignal(HWND from, UINT signal)
{
    return 0;
}
bool         IPCWindow::IsConfirmedPartner(HWND wnd) {
    return ExtendedGetState() >= (unsigned int)IPCWCode::HandshakeComplete && partner == wnd;
}


void IPCWindow::ExtendedSetState(unsigned int state) {
    if(state < IPCWCode::ExtendedCodeStart)
        throw "Use ExtendedCodeStart as the minimum value for extended states";
    this->state = static_cast<IPCWCode>(state);
}
unsigned int IPCWindow::ExtendedGetState() {
    return static_cast<unsigned int>(state);
}

void IPCWindow::RunAfterCreated(void (*handler)(IPCWindow* createdWindow), bool runIfAlreadyCreated) {
    if(state == None)
        RegisterLifecycleEvent(handler);
    else if(runIfAlreadyCreated)
        handler(this);
}
void IPCWindow::RunAfterHandshake(void (*handler)(IPCWindow* shakenWindow), bool runIfAlreadyShaken)
{
    if(state == None)
        throw "Let the window be created before adding these events. You can add these events inside RunAfterCreated(...)";
    if(!IsConfirmedPartner(partner))
        RegisterLifecycleEvent(handler);
    else if(runIfAlreadyShaken)
        handler(this);
}

void IPCWindow::RunOnce(void (*handler)(IPCWindow* shakenWindow), unsigned int wait_ms) {
    if(!IsConfirmedPartner(partner))
        throw "RunOnce not available until after handshake";
    RegisterLifecycleEvent(handler);
    KillTimer(self, IPCWCode::Timer_RunOnceWhenEverySubmissionIsReady);
    SetTimer(self, IPCWCode::Timer_RunOnceWhenEverySubmissionIsReady, wait_ms, nullptr);
}

void IPCWindow::RegisterLifecycleEvent(void (*eventHandler)(IPCWindow*)) { 
    LifecycleEvent = new std::pair<void (*)(IPCWindow*), void*>(eventHandler, LifecycleEvent);
}

#include <vector>

void IPCWindow::TriggerLifecycleEvents(bool retainEvents) { 
    auto history = std::vector<void (*)(IPCWindow*)>();
    auto run     = [&](void (*f)(IPCWindow*)) { 
        if(std::find(history.begin(), history.end(), f) == history.end())
        {
            history.push_back(f);
            f(this);
        }
    };

    if(retainEvents)
        for(auto e = LifecycleEvent; e; e = (std::pair<void (*)(IPCWindow*), void*>*)e->second)
            run(e->first);
    else
    {
        auto e         = LifecycleEvent;
        LifecycleEvent = nullptr;
        while(e)
        {
            run(e->first);
            auto n = (std::pair<void (*)(IPCWindow*), void*>*)e->second;
            delete e;
            e = n;
        }
    }
}
void IPCWindow::ClearLifecycleEvents() { 
    
    auto e         = LifecycleEvent;
    LifecycleEvent = nullptr;
    while(e)
    {
        auto n = (std::pair<void (*)(IPCWindow*), void*>*)e->second;
        delete e;
        e = n;
    }
}

void IPCWindow::SetHandshakeCompletedState() 
{
    state = IPCWCode::HandshakeComplete;
    auto criteria = GetStatedata<PartnerCriteria>();
    if(!CheckWindowStrings(partner, std::get<2>(criteria), std::get<1>(criteria)))
        goto fail;
    if(std::get<0>(criteria) < 1)
    {
        DWORD wpid;
        if(!GetWindowThreadProcessId(partner, &wpid))
            goto fail;
        std::get<0>(criteria) = wpid;
        SetStatedataFromMemBlock(&criteria);
    }
    else if(!CheckWindowPid(partner, std::get<0>(criteria)))
        goto fail;
    return;

    fail:
    state = IPCError;
}

bool IPCWindow::ReverifyPartnerCriteria(PartnerCriteria& criteria) {
    #define candidate partner
    return CHECK_WINDOW;
}
bool IPCWindow::ReverifyPartnerCriteriaInStatedataPtr() {
    auto c = (PartnerCriteria*)statedata;
    return c ? ReverifyPartnerCriteria(*c) : false;
}

bool IPCWindow::IsWindowDestroyed()
{
    return self == NULL && state;
}
bool IPCWindow::ResetToNewlyCreatedState(bool force) {
    switch (state)
    {
    case None:
        return false;
    case CompletingHandshake:
    case CompletingHandshakeResponse:
        if(!force)
            return false;
        SendSignal(IPCWCode::IPCError);
        break;
    default:
        if(state >= IPCWCode::HandshakeComplete)
        {
            if(!force)
                return false;
            state = Unspecified;
            SendSignal(IPCWCode::PartnerShuttingDown);
        }
    }
    partner = nullptr;
    SetStatedataFromMemBlock<PartnerCriteria>(nullptr);
    ClearLifecycleEvents();
    if(IsWindowDestroyed())
    {
        state = None;
        return false;
    }
    state = NewlyCreated;
    return true;
}

HWND IPCWindow::GetHWND()
{
    return self;
}