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

using System.Net.Sockets;
using System.Text;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Midjourney.Base
{
    /// <summary>
    /// HttpContext 扩展
    /// </summary>
    public static class HttpContextExtensions
    {
        /// <summary>
        /// 获取请求链接地址
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public static string GetUrl(this HttpRequest httpRequest)
        {
            if (httpRequest == null)
            {
                return string.Empty;
            }

            return new StringBuilder()
                .Append(httpRequest.Scheme).Append("://")
                .Append(httpRequest.Host).Append(httpRequest.PathBase)
                .Append(httpRequest.Path).Append(httpRequest.QueryString).ToString();
        }

        /// <summary>
        /// 获取客户端 IP 地址
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <param name="ignoreLocalIpAddress">验证时是否忽略本地 IP 地址，如果忽略本地 IP 地址，则当判断为本地 IP 地址时返回可能为空</param>
        /// <returns></returns>
        public static string GetIP(this HttpRequest httpRequest, bool ignoreLocalIpAddress = false)
        {
            if (httpRequest == null)
            {
                return string.Empty;
            }

            var ip = string.Empty;
            var headers = httpRequest.Headers;

            // 可以被伪造（使用百度云加速时，百度云会自动移除此客户端请求头，且不可被伪造）
            // 获取True-Client-Ip头信息（百度云加速用户真实 IP）
            if (string.IsNullOrWhiteSpace(ip))
            {
                if (httpRequest.Headers.ContainsKey("True-Client-Ip"))
                {
                    if (httpRequest.Headers.TryGetValue("True-Client-Ip", out var tci))
                    {
                        ip = tci.ToString();
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(ip))
            {
                if (httpRequest.Headers.ContainsKey("X-Real-IP"))
                {
                    ip = httpRequest.Headers["X-Real-IP"].FirstOrDefault();
                }
            }

            if (string.IsNullOrWhiteSpace(ip))
            {
                if (httpRequest.Headers.ContainsKey("X-Forwarded-For"))
                {
                    ip = httpRequest.Headers["X-Forwarded-For"].FirstOrDefault();
                }
            }

            if (string.IsNullOrEmpty(ip))
            {
                var address = httpRequest.HttpContext.Connection.RemoteIpAddress;

                // compare with local address
                if (ignoreLocalIpAddress && address == httpRequest.HttpContext.Connection.LocalIpAddress)
                {
                    ip = string.Empty;
                }

                if (address?.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    ip = address?.MapToIPv4()?.ToString();
                }

                if (string.IsNullOrWhiteSpace(ip))
                {
                    ip = address?.ToString();
                }
            }

            if (string.IsNullOrWhiteSpace(ip))
            {
                ip = httpRequest.Host.Host ?? httpRequest.Host.Value;
            }

            if (!string.IsNullOrWhiteSpace(ip) && ip.Contains(","))
            {
                ip = ip.Split(',')[0];
            }

            // 如果用户 IP 无法获取，则固定为 1.1.1.1
            if (string.IsNullOrEmpty(ip))
            {
                Log.Logger.Warning("IP address is empty, set to 1.1.1.1");

                ip = "1.1.1.1";
            }

            //var context = httpRequest.HttpContext;
            //var request = context.Request;
            //var ipInfo = new
            //{
            //    RemoteIp = context.Connection.RemoteIpAddress?.ToString(),
            //    XForwardedFor = request.Headers["X-Forwarded-For"],
            //    XRealIp = request.Headers["X-Real-IP"],
            //    XOriginalFor = request.Headers["X-Original-For"],
            //    CFConnectingIp = request.Headers["CF-Connecting-IP"], // Cloudflare
            //    XClientIp = request.Headers["X-Client-IP"],
            //    UserAgent = request.Headers["User-Agent"]
            //};

            //Log.Debug("Request IP Info: {Method} {Path} - {@IpInfo}",
            //    request.Method,
            //    request.Path,
            //    ipInfo);

            return ip;
        }
    }
}