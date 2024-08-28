﻿using SQLOperation.DataAccessLayer.ManagementFeatureDAL;
using SQLOperation.PublicAccess.Utilities;
using SQLOperation.PublicAccess.Utilities.ManagementFeatureUtil;
using System.Text.Json;

namespace SQLOperation.BusinessLogicLayer.ManagementFeatureBLL
{
    public class GetUserInfoBLL
    {
        private UserOperatorDAL UserOperatorDAL;
        public GetUserInfoBLL() 
        { 
            UserOperatorDAL = new UserOperatorDAL();
        }

        public Tuple<bool, string> GetInfo(int? UserID, string? UserName, bool IsAdmin = false)
        {
            Tuple<bool, string> QueryResult = UserOperatorDAL.GetUserInfo(UserID, UserName);

            if (!QueryResult.Item1)
                return new Tuple<bool, string>(false, QueryResult.Item2);

            var users = JsonSerializer.Deserialize<List<Users>>(QueryResult.Item2);

            if (users == null || users.Count <= 0)
                return new Tuple<bool, string>(false, "无法将Users类型转换为Json类型");

            List<UserAccessibleInfo> Resultlist = [];

            foreach (var user in users)
            {
                if (!IsAdmin && user.Is_Deleted == 1)
                    continue;

                Resultlist.Add(new UserAccessibleInfo()
                {
                    UserName = user.User_Name,
                    UserID = user.User_ID,
                    Contact = user.Contact,
                    IsDeleted = user.Is_Deleted
                });
            }

            return new Tuple<bool, string>(true, JsonSerializer.Serialize(Resultlist));
        }
    }
}
