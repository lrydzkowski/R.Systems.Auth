﻿namespace R.Systems.Auth.WebApi.Features.User;

public class ChangePasswordRequest
{
    public string OldPassword { get; set; } = "";

    public string NewPassword { get; set; } = "";

    public string RepeatedNewPassword { get; set; } = "";
}
