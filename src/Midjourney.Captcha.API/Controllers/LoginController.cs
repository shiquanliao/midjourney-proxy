﻿// Midjourney Proxy - Proxy for Midjourney's Discord, enabling AI drawings via API with one-click face swap. A free, non-profit drawing API project.
// Copyright (C) 2024 trueai.org

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

// Additional Terms:
// This software shall not be used for any illegal activities.
// Users must comply with all applicable laws and regulations,
// particularly those related to image and video processing.
// The use of this software for any form of illegal face swapping,
// invasion of privacy, or any other unlawful purposes is strictly prohibited.
// Violation of these terms may result in termination of the license and may subject the violator to legal action.

using Microsoft.AspNetCore.Mvc;

namespace Midjourney.Captcha.API.Controllers
{
    /// <summary>
    /// 自动登录控制器。
    /// </summary>
    [Route("login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        /// <summary>
        /// 自动登录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("auto")]
        public ActionResult AutoLogin([FromBody] AutoLoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.State))
            {
                return BadRequest("State 不能为空");
            }

            if (string.IsNullOrWhiteSpace(request.LoginAccount) || string.IsNullOrWhiteSpace(request.LoginPassword))
            {
                return BadRequest("账号或密码不能为空");
            }

            if (string.IsNullOrWhiteSpace(request.Login2fa))
            {
                return BadRequest("2FA 密钥不能为空");
            }

            SeleniumLoginQueueHostedService.EnqueueRequest(request);

            return Ok();
        }
    }
}