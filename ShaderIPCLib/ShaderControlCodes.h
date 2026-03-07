#pragma once

#include "IPCWindow.h"

enum ShaderControlCode
{
	NewProfilePath = IPCWCode::ExtendedCodeStart,
	ReloadProfile,
	SaveProfile,
	NewShaderPath,
	ResetShaderParams,
    AddTooltipToNewParameter,
	ParameterData,
	FullscreenOn,
	FullscreenOff,
	ToggleMenu,
	Minimize,
	Restore,
	BeginRedefine,
	EndRedefine
};