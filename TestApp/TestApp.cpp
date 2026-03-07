// TestApp.cpp : Defines the entry point for the application.
//

#include "framework.h"
#include "TestApp.h"
#include "IPCWindow.h"


#define MAX_LOADSTRING 100

// Global Variables:
HINSTANCE hInst;                                // current instance
WCHAR szTitle[MAX_LOADSTRING];                  // The title bar text
WCHAR szWindowClass[MAX_LOADSTRING];            // the main window class name

auto szButtonCreateChild = L"Create &child";
auto szButtonContactParent= L"Contact &parent";
HWND hwndButton, hwndTrace, m_hwnd;

LPCWSTR aschild = L"-aschild";
IPCWindow ipc     = IPCWindow();

// Forward declarations of functions included in this code module:
ATOM                MyRegisterClass(HINSTANCE hInstance);
HWND InitInstance(HINSTANCE, int);
LRESULT CALLBACK    WndProc(HWND, UINT, WPARAM, LPARAM);
INT_PTR CALLBACK    About(HWND, UINT, WPARAM, LPARAM);

void CreateChild();
void ContactParent();

int APIENTRY wWinMain(_In_ HINSTANCE hInstance,
                     _In_opt_ HINSTANCE hPrevInstance,
                     _In_ LPWSTR    lpCmdLine,
                     _In_ int       nCmdShow)
{
    UNREFERENCED_PARAMETER(hPrevInstance);
    UNREFERENCED_PARAMETER(lpCmdLine);

    auto button_text = szButtonCreateChild;

    // TODO: Place code here.
    {
        LPWSTR* szArgList;
        int     argCount;

        szArgList = CommandLineToArgvW(GetCommandLine(), &argCount);
        if(szArgList == NULL)
        {
            MessageBox(NULL, L"Unable to parse command line", L"Error", MB_OK);
            return 10;
        }

        for(int i = 0; i < argCount; i++)
        {
            if(wcscmp(szArgList[i], aschild) == 0)
            {
                button_text = szButtonContactParent;
                szButtonCreateChild = nullptr;
            }
        }

        LocalFree(szArgList);
    }

    // Initialize global strings
    LoadStringW(hInstance, IDS_APP_TITLE, szTitle, MAX_LOADSTRING);
    LoadStringW(hInstance, IDC_TESTAPP, szWindowClass, MAX_LOADSTRING);
    MyRegisterClass(hInstance);

    // Perform application initialization:
    m_hwnd = InitInstance(hInstance, nCmdShow);
    if(!m_hwnd)
    {
        return FALSE;
    }

    ipc.CustomRef = &m_hwnd;
    ipc.OnPartnerShuttingDown = [](IPCWindow* win) { 
        DestroyWindow(*(HWND*)win->CustomRef);
        return (LRESULT)0;
    };
    

    HACCEL hAccelTable = LoadAccelerators(hInstance, MAKEINTRESOURCE(IDC_TESTAPP));

    MSG msg;

    hwndButton = CreateWindow(L"BUTTON", // Predefined class; Unicode assumed
                                   button_text,
                                   WS_TABSTOP | WS_VISIBLE | WS_CHILD | BS_DEFPUSHBUTTON, // Styles
                                   10, // x position
                                   10, // y position
                                   100, // Button width
                                   100, // Button height
                                   m_hwnd, // Parent window
                                   NULL, // No menu.
                                   (HINSTANCE)GetWindowLongPtr(m_hwnd, GWLP_HINSTANCE),
                                   NULL); // Pointer not needed.

    hwndTrace =  CreateWindowEx(WS_EX_CLIENTEDGE, L"STATIC", L"MESSAGE TRACE", WS_VISIBLE | WS_CHILD | WS_BORDER | ES_LEFT, 100, 10, 1400, 680, m_hwnd, NULL, hInstance, NULL);

    SetTimer(m_hwnd, 1, 3000, nullptr);

    // Main message loop:
    while (GetMessage(&msg, nullptr, 0, 0))
    {
        if (!TranslateAccelerator(msg.hwnd, hAccelTable, &msg))
        {
            TranslateMessage(&msg);
            DispatchMessage(&msg);
        }
    }

    return (int) msg.wParam;
}



//
//  FUNCTION: MyRegisterClass()
//
//  PURPOSE: Registers the window class.
//
ATOM MyRegisterClass(HINSTANCE hInstance)
{
    WNDCLASSEXW wcex;

    wcex.cbSize = sizeof(WNDCLASSEX);

    wcex.style          = CS_HREDRAW | CS_VREDRAW;
    wcex.lpfnWndProc    = WndProc;
    wcex.cbClsExtra     = 0;
    wcex.cbWndExtra     = 0;
    wcex.hInstance      = hInstance;
    wcex.hIcon          = LoadIcon(hInstance, MAKEINTRESOURCE(IDI_TESTAPP));
    wcex.hCursor        = LoadCursor(nullptr, IDC_ARROW);
    wcex.hbrBackground  = (HBRUSH)(COLOR_WINDOW+1);
    wcex.lpszMenuName   = MAKEINTRESOURCEW(IDC_TESTAPP);
    wcex.lpszClassName  = szWindowClass;
    wcex.hIconSm        = LoadIcon(wcex.hInstance, MAKEINTRESOURCE(IDI_SMALL));

    return RegisterClassExW(&wcex);
}

//
//   FUNCTION: InitInstance(HINSTANCE, int)
//
//   PURPOSE: Saves instance handle and creates main window
//
//   COMMENTS:
//
//        In this function, we save the instance handle in a global variable and
//        create and display the main program window.
//
HWND InitInstance(HINSTANCE hInstance, int nCmdShow)
{
   hInst = hInstance; // Store instance handle in our global variable

   HWND hWnd = CreateWindowW(szWindowClass, szTitle, WS_OVERLAPPEDWINDOW,
      CW_USEDEFAULT, 0, CW_USEDEFAULT, 0, nullptr, nullptr, hInstance, nullptr);

   if(hWnd)
   {
       ShowWindow(hWnd, nCmdShow);
       UpdateWindow(hWnd);
   }
   return hWnd;
}

//
//  FUNCTION: WndProc(HWND, UINT, WPARAM, LPARAM)
//
//  PURPOSE: Processes messages for the main window.
//
//  WM_COMMAND  - process the application menu
//  WM_PAINT    - Paint the main window
//  WM_DESTROY  - post a quit message and return
//
//


#include <string>
#include <vector>

auto traceText = std::wstring();
auto traceTextNew = std::wstring();
auto tracePrior   = std::vector<std::tuple<HWND, UINT, WPARAM>>();
int  lbr          = 0, lbrm = -1;
HWND lastTraced = NULL;

LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    if(message != WM_COPYDATA && message != WM_TIMER && hWnd != hwndButton && hWnd != hwndTrace)
    {
        if(lbrm < 0)
        {
            if(message == WM_SYSCOMMAND && wParam == SC_MINIMIZE)
                lbrm = 0;
        }
        if(lbrm > -1)
        {

            auto filter = std::tuple<HWND, UINT, WPARAM>(hWnd, message, wParam);

            if(std::find(tracePrior.begin(), tracePrior.end(), filter) == tracePrior.end())
            {
                tracePrior.push_back(filter);
                std::wstring s;

                if(hWnd == lastTraced)
                {
                    traceTextNew.append(L"&&");
                }
                else
                {
                    traceTextNew.append(hWnd == m_hwnd ? L"Window = " : L"HWND = ");
                    s = std::to_wstring((unsigned long long)hWnd);
                    traceTextNew.append(s);
                    lastTraced = hWnd;
                    lbr++;
                }
                s = std::to_wstring(message);
                traceTextNew.append(L" <= ").append(s).append(L"(");
                s = std::to_wstring(wParam);
                traceTextNew.append(s).append(L", ");
                s = std::to_wstring(lParam);
                traceTextNew.append(s);
                if(++lbr > 5)
                {
                    lbr = 0;
                    traceTextNew.append(L")\r\n");
                    lbrm++;
                }
                else
                    traceTextNew.append(L")\t");
            }
        }
    }

    switch (message)
    {
    case WM_TIMER:
        if(traceTextNew.length())
        {
            if(lbrm > 30)
            {
                traceText.clear();
                lbrm = 0;
            }

            traceText.append(traceTextNew);
            SetWindowText(hwndTrace, traceText.c_str());
            traceTextNew.clear();
        }
        break;

    case WM_COMMAND:
        {
            int wmId = LOWORD(wParam);
            // Parse the menu selections:
            switch (wmId)
            {
            case IDM_ABOUT:
                DialogBox(hInst, MAKEINTRESOURCE(IDD_ABOUTBOX), hWnd, About);
                break;
            case IDM_EXIT:
                DestroyWindow(hWnd);
                break;
            default:
                if((HWND)lParam == hwndButton)
                {
                    if(szButtonCreateChild)
                        CreateChild();
                    else
                        ContactParent();
                }
                else return DefWindowProc(hWnd, message, wParam, lParam);
                return DefWindowProc(hWnd, WM_SYSCOMMAND, SC_MINIMIZE, 0);
            }
        }
        break;
    case WM_PAINT:
        {
            PAINTSTRUCT ps;
            HDC hdc = BeginPaint(hWnd, &ps);
            // TODO: Add any drawing code that uses hdc here...
            EndPaint(hWnd, &ps);
        }
        break;
    case WM_DESTROY:
        DestroyWindow(ipc.GetHWND());
        PostQuitMessage(0);        
        break;
    default:
        return DefWindowProc(hWnd, message, wParam, lParam);
    }
    return 0;
}

// Message handler for about box.
INT_PTR CALLBACK About(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam)
{
    UNREFERENCED_PARAMETER(lParam);
    switch (message)
    {
    case WM_INITDIALOG:
        return (INT_PTR)TRUE;

    case WM_COMMAND:
        if (LOWORD(wParam) == IDOK || LOWORD(wParam) == IDCANCEL)
        {
            EndDialog(hDlg, LOWORD(wParam));
            return (INT_PTR)TRUE;
        }
        break;
    }
    return (INT_PTR)FALSE;
}

void CreateChild() 
{
    if(ipc.ExtendedGetState() != IPCWCode::None)
        return;

     ipc.RunAfterCreated([](IPCWindow* ipc) { 
         ipc->AnticipateHandshake();
         ipc->RunAfterHandshake([](IPCWindow* ipc) { SetWindowText(hwndButton, L"COMPLETED"); });
     });

    DWORD size = _MAX_PATH;
retry:
    LPWSTR exe = new WCHAR[size];
    SetLastError(ERROR_SUCCESS);
    size = GetModuleFileNameW(nullptr, exe, size);
    if(GetLastError() == ERROR_INSUFFICIENT_BUFFER)
    {
        delete[] exe;
        size *= 2;
        goto retry;
    }

    WCHAR args[100];
    wcscpy_s(args, sizeof(args) / sizeof(WCHAR), aschild);
    STARTUPINFO         si;
    PROCESS_INFORMATION pi;

    // set the size of the structures
    ZeroMemory(&si, sizeof(si));
    si.cb = sizeof(si);
    ZeroMemory(&pi, sizeof(pi));
    CreateProcessW(exe, args, nullptr, nullptr, true, 0, nullptr, nullptr, &si, &pi);

    ipc.SetPartnerDetails(pi.dwProcessId);

    CloseHandle(pi.hProcess);
    CloseHandle(pi.hThread);   

    ipc.Create(hInst);
}
void ContactParent() 
{
    if(ipc.ExtendedGetState() != IPCWCode::None)
        return;

    ipc.RunAfterCreated([](IPCWindow* ipc) {
        ipc->BeginShakingHandsUntilSuccessful();
        ipc->RunAfterHandshake([](IPCWindow* ipc) {
            SetWindowText(hwndButton, L"COMPLETED");
        });
    });
    ipc.Create(hInst);
}

/*
class EventHandler : public IUIAutomationEventHandler
{
private:
    LONG _refCount;

public:
    int _eventCount;

    // Constructor.
    EventHandler() : _refCount(1), _eventCount(0) { }

    // IUnknown methods.
    ULONG STDMETHODCALLTYPE AddRef()
    {
        ULONG ret = InterlockedIncrement(&_refCount);
        return ret;
    }

    ULONG STDMETHODCALLTYPE Release()
    {
        ULONG ret = InterlockedDecrement(&_refCount);
        if(ret == 0)
        {
            delete this;
            return 0;
        }
        return ret;
    }

    HRESULT STDMETHODCALLTYPE QueryInterface(REFIID riid, void** ppInterface)
    {
        if(riid == __uuidof(IUnknown))
            *ppInterface = static_cast<IUIAutomationEventHandler*>(this);
        else if(riid == __uuidof(IUIAutomationEventHandler))
            *ppInterface = static_cast<IUIAutomationEventHandler*>(this);
        else
        {
            *ppInterface = NULL;
            return E_NOINTERFACE;
        }
        this->AddRef();
        return S_OK;
    }

    // IUIAutomationEventHandler methods
    HRESULT STDMETHODCALLTYPE HandleAutomationEvent(IUIAutomationElement* pSender, EVENTID eventID)
    {
        _eventCount++;
        switch(eventID)
        {
        case UIA_ToolTipOpenedEventId:
            wprintf(L">> Event ToolTipOpened Received! (count: %d)\n", _eventCount);
            break;
        case UIA_ToolTipClosedEventId:
            wprintf(L">> Event ToolTipClosed Received! (count: %d)\n", _eventCount);
            break;
        case UIA_Window_WindowOpenedEventId:
            wprintf(L">> Event WindowOpened Received! (count: %d)\n", _eventCount);
            break;
        case UIA_Window_WindowClosedEventId:
            wprintf(L">> Event WindowClosed Received! (count: %d)\n", _eventCount);
            break;
        default:
            wprintf(L">> Event (%d) Received! (count: %d)\n", eventID, _eventCount);
            break;
        }
        return S_OK;
    }
};
*/