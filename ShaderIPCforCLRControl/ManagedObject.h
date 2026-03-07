#pragma once
//#include "Wrapper.h"

// https://www.red-gate.com/simple-talk/development/dotnet-development/creating-ccli-wrapper/

using namespace System;
namespace ShaderIPCforCLRControl
{

template <class T>
public ref class ManagedObject//: Wrapper
{
private:
    T* m_Instance;
protected:
    /* void* GetUnmanagedPointer() override
    {
        return m_Instance;
    }*/
    T* GetInstance() {
        return m_Instance;
    }

    virtual void OnDispose(T* disposeInstance)
    { 
    }

public:
    ManagedObject(T* instance) //: Wrapper(instance)
    {
        m_Instance = instance;
    }
    virtual ~ManagedObject()
    {
        auto m     = m_Instance;
        if(m != nullptr)
        {
            //CallBeforeLosingPointer();
            m_Instance = nullptr;
            OnDispose(m);
            delete m;
        }
    }
    !ManagedObject()
    {
        auto m = m_Instance;
        if(m != nullptr)
        {
            //CallBeforeLosingPointer();
            m_Instance = nullptr;
            OnDispose(m);
            delete m;
        }
    }
};
} // namespace ShaderIPCforCLRControl