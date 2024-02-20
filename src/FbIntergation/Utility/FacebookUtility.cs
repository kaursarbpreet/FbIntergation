using Facebook;
using Microsoft.Extensions.Options;
using NHLytics.Models;

namespace NHlytics.Utility
{
    public class FacebookUtility
    {
        private readonly FbSettings _settings;

        public FacebookUtility(IOptions<FbSettings> fbSettings)
        {
            _settings = fbSettings.Value;
        }

        public Uri GetloginUrl()
        {
            var fb = new FacebookClient();
            return fb.GetLoginUrl(new
            {
                client_id = _settings.client_id,
                redirect_uri = _settings.redirect_uri,
                scope = "email,pages_show_list,pages_manage_posts,pages_manage_metadata,pages_manage_cta" +
                ",page_events,pages_read_user_content,pages_read_engagement,pages_manage_engagement,business_management"
            });
        }

        public FbModel FacebookRedirect(string code)
        {
            var accessToken = this.AccessTokenGet(code);
            var fbClient = new FacebookClient(accessToken);
            dynamic fbclientdetail = fbClient.Get("/me?fields=id,name,email,first_name,last_name,age_range,birthday,gender,locale");

            FbModel fbModel = new FbModel();
            fbModel.AccessToken = accessToken;
            fbModel.Expires_at = DateTime.Now.AddDays(10);
            fbModel.FirstName = fbclientdetail.first_name;
            fbModel.LastName = fbclientdetail.last_name;
            fbModel.Email = fbclientdetail.email;
            fbModel.AppScopedUserId = fbclientdetail.id;

            var pagesInformation = this.PagesInformationGet(accessToken, fbclientdetail.id);
            fbModel.Pages = this.FormatPageInformation(pagesInformation);

            return fbModel;
        }

        public FbModel GetdataWithToken(string token)
        {
            FbModel returModel = new FbModel();
            returModel.Pages = GetCompanyPages(token);

            return returModel;
        }

        public PagesResponse GetCompanyPages(string token)
        {
            var fb = new FacebookClient(token);
            PagesResponse pages = new PagesResponse();
            pages.Data = new List<Page>();

            dynamic results = fb.Get("/me/accounts");
            foreach (var data in results.data)
            {
                Page page = new Page();
                page.Category = data.category;
                page.Name = data.name;
                page.Id = data.id;

                pages.Data.Add(page);
            }

            return pages;
        }


        private string AccessTokenGet(string code)
        {
            var fb = new FacebookClient();

            dynamic result = fb.Get("/oauth/access_token", new
            {
                client_id = _settings.client_id,
                client_secret = _settings.client_secret,
                redirect_uri = _settings.redirect_uri,
                code = code
            });

            return result.access_token;
        }

        private dynamic PagesInformationGet(string accessToken, string userId)
        {
            var fb = new FacebookClient(accessToken);
            dynamic resultlongLived = fb.Get("/oauth/access_token", new
            {
                grant_type = "fb_exchange_token",
                client_id = _settings.client_id,
                client_secret = _settings.client_secret,
                redirect_uri = _settings.redirect_uri,
                fb_exchange_token = accessToken
            });

            return fb.Get($"/{userId}/accounts", new
            {
                access_token = resultlongLived.access_token
            });
        }

        private PagesResponse FormatPageInformation(dynamic pageInformation)
        {
            PagesResponse pages = new PagesResponse();
            pages.Data = new List<Page>();

            foreach (var data in pageInformation.data)
            {
                Page page = new Page();
                page.Token = data.access_token;
                page.Category = data.category;
                page.Name = data.name;
                page.Id = data.id;

                pages.Data.Add(page);
            }
            return pages;
        }
    }
}