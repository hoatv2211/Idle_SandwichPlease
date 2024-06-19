using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

public class UserManager : NekoMart.Utils.Singleton<UserManager>
{
    public enum UserGroup
    {
        free_guest,
        free_synced_wallet,
        NFT_no_premium_battle,
        NFT_joined_premium_battle
    }

    public class UserData
    {
        public long UserId = 1;

        public bool IsFreeUser;
        public bool IsNewUser;
        public string Email;
        
        private string _username;
        public string UserName
        {
            set
            {
                _username = value;
            }
            get
            {
                return string.IsNullOrEmpty(_username) ? "Guest" : _username;
            }
        }
        public string InviteCode = "";
       

        public UserData Clone()
        {
            return this.MemberwiseClone() as UserData;
        }
    }

    private bool _isLoadClaimCache = false;
    private bool _isLoadTimeCache = false;

    private UserData data = new UserData();

    public UserData Data
    {
        get
        {
            return data;
        }
    }

    public static UserData IData
    {
        get
        {
            return Instance.Data;
        }
    }

}