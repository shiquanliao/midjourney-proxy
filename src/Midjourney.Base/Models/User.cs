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

using FreeSql.DataAnnotations;
using Midjourney.Base.Data;
using MongoDB.Bson.Serialization.Attributes;

namespace Midjourney.Base.Models
{
    /// <summary>
    /// 用户
    /// </summary>
    [BsonCollection("user")]
    [MongoDB.Bson.Serialization.Attributes.BsonIgnoreExtraElements]
    [Serializable]
    public class User : DomainObject
    {
        public User()
        {
        }

        /// <summary>
        /// 用户昵称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        [Column(StringLength = 2000)]
        public string Avatar { get; set; }

        /// <summary>
        /// 角色 ADMIN | USER
        /// </summary>
        public EUserRole? Role { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public EUserStatus? Status { get; set; }

        /// <summary>
        /// 用户令牌
        /// </summary>
        [Column(StringLength = 2000)]
        public string Token { get; set; }

        /// <summary>
        /// 最后登录 ip
        /// </summary>
        public string LastLoginIp { get; set; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? LastLoginTime { get; set; }

        /// <summary>
        /// 最后登录时间格式化
        /// </summary>
        public string LastLoginTimeFormat => LastLoginTime?.ToString("yyyy-MM-dd HH:mm");

        /// <summary>
        /// 注册 ip
        /// </summary>
        public string RegisterIp { get; set; }

        /// <summary>
        /// 注册时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime RegisterTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 注册时间格式化
        /// </summary>
        public string RegisterTimeFormat => RegisterTime.ToString("yyyy-MM-dd HH:mm");

        /// <summary>
        /// 日绘图最大次数限制，默认 0 不限制
        /// </summary>
        public int DayDrawLimit { get; set; } = -1;

        /// <summary>
        /// 用户最大绘图数量限制，默认 0 不限制
        /// </summary>
        public int TotalDrawLimit { get; set; } = -1;

        /// <summary>
        /// 今日绘图次数（数据库字段已废弃）
        /// </summary>
        [LiteDB.BsonIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        [Column(IsNullable = false)]
        public int DayDrawCount { get; set; } = 0;

        /// <summary>
        /// 总绘图次数（数据库字段已废弃）
        /// </summary>
        [LiteDB.BsonIgnore]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        [Column(IsNullable = false)]
        public int TotalDrawCount { get; set; } = 0;

        /// <summary>
        /// 用户并发绘图数量限制，默认 -1 不限制
        /// </summary>
        public int CoreSize { get; set; } = -1;

        /// <summary>
        /// 用户队列绘图数量限制，默认 -1 不限制
        /// </summary>
        public int QueueSize { get; set; } = -1;

        /// <summary>
        /// 白名单用户（加入白名单不受限流控制）
        /// </summary>
        public bool IsWhite { get; set; } = false;

        /// <summary>
        /// 账号有效开始时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? ValidStartTime { get; set; }

        /// <summary>
        /// 账号有效开始时间
        /// </summary>
        public string ValidStartTimeFormat => ValidStartTime?.ToString("yyyy-MM-dd");

        /// <summary>
        /// 账号有效结束时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? ValidEndTime { get; set; }

        /// <summary>
        /// 账号有效结束时间
        /// </summary>
        public string ValidEndTimeFormat => ValidEndTime?.ToString("yyyy-MM-dd");

        /// <summary>
        /// 账号是否可用（在有效期内）
        /// </summary>
        public bool IsAvailable => (ValidStartTime == null || ValidStartTime <= DateTime.Now)
            && (ValidEndTime == null || ValidEndTime >= DateTime.Now);

        /// <summary>
        /// 创建时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 更新时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UpdateTime { get; set; } = DateTime.Now;
    }
}