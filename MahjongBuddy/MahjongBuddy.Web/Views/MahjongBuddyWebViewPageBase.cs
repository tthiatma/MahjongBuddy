using Abp.Web.Mvc.Views;

namespace MahjongBuddy.Web.Views
{
    public abstract class MahjongBuddyWebViewPageBase : MahjongBuddyWebViewPageBase<dynamic>
    {

    }

    public abstract class MahjongBuddyWebViewPageBase<TModel> : AbpWebViewPage<TModel>
    {
        protected MahjongBuddyWebViewPageBase()
        {
            LocalizationSourceName = MahjongBuddyConsts.LocalizationSourceName;
        }
    }
}