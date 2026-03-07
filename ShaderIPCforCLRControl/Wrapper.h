#pragma once

namespace ShaderIPCforCLRControl
{

public ref class Wrapper abstract
{ 
	protected:
    virtual void* GetUnmanagedPointer() = 0;
    Wrapper(void* ptr) {
        if(!map->TryAdd((System::IntPtr)ptr, this))
            throw gcnew System::Exception("could not create link back from unmanaged");
        }

    void CallBeforeLosingPointer() {
        auto ptr = GetUnmanagedPointer();
        if(ptr)
        {
            Wrapper ^ w;
            map->TryRemove((System::IntPtr)ptr, w);
        }
    }
    
    static Wrapper^ GetWrapperFromPointer(void* ptr)
    {
        Wrapper ^ w;
        if(ptr)
            return map->TryGetValue((System::IntPtr)ptr, w) ? w : nullptr;
        return nullptr;
    }

	private:
    static System::Collections::Concurrent::ConcurrentDictionary<System::IntPtr, Wrapper ^> ^ map = gcnew System::Collections::Concurrent::ConcurrentDictionary<System::IntPtr, Wrapper ^>();
};

} // namespace ShaderIPCforCLRControl