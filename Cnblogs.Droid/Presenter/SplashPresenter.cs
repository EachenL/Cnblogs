using Android.Content;
using Cnblogs.Droid.Model;
using Cnblogs.Droid.UI.Shareds;
using Cnblogs.Droid.UI.Views;
using Cnblogs.Droid.Utils;
using Java.Util.Concurrent;
using Newtonsoft.Json;
using Square.OkHttp3;
using System;
using System.Collections.Generic;

namespace Cnblogs.Droid.Presenter
{
    public class SplashPresenter : ISplashPresenter
    {
        private ISplashView splashView;
        private Context context;
        public SplashPresenter(Context context, ISplashView splashView)
        {
            this.context = context;
            this.splashView = splashView;
        }
        public void GetAccessToken(AccessToken token, string basic)
        {
            try
            {
                var param = new List<OkHttpUtils.Param>()
                {
                    new OkHttpUtils.Param("grant_type","client_credentials")
                };
                OkHttpUtils.Instance(token).Post(ApiUtils.Token, basic, param, async (call, response) =>
                 {
                     var code = response.Code();
                     var body = await response.Body().StringAsync();
                     if (code == (int)System.Net.HttpStatusCode.OK)
                     {
                         token = JsonConvert.DeserializeObject<AccessToken>(body);
                         token.RefreshTime = DateTime.Now;

                         TokenShared.Update(context, token);
                     }
                     else
                     {
                         TokenShared.Update(context, new AccessToken());
                     }
                 }, (call, ex) =>
                 {
                 });
            }
            catch (Exception ex)
            {
            }
        }
        public void UserRefreshToken(AccessToken token, string basic)
        {
            try
            {
                var param = new List<OkHttpUtils.Param>()
                {
                    new OkHttpUtils.Param("grant_type","refresh_token"),
                    new OkHttpUtils.Param("refresh_token",token.refresh_token)
                };

                OkHttpUtils.Instance(token).Post(ApiUtils.Token, basic, param, async (call, response) =>
                {
                    var code = response.Code();
                    var body = await response.Body().StringAsync();
                    if (code == (int)System.Net.HttpStatusCode.OK)
                    {
                        token = JsonConvert.DeserializeObject<AccessToken>(body);
                        token.RefreshTime = DateTime.Now;

                        UserShared.Update(context, token);
                    }
                    else
                    {
                        UserShared.Update(context, new AccessToken());
                    }
                }, (call, ex) =>
                {
                });
            }
            catch (Exception ex)
            {
            }
        }
    }
}