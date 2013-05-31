using System.Web.Optimization;

namespace Web.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/bundles/site")
                            .Include("~/Content/stylesheet.css",
                                     "~/Content/pygment_trac.css",
                                     "~/Content/prettify.css",
                                     "~/Content/sprint.grid.css"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/javascript").Include(
                       "~/Scripts/sprint.grid/sprint.grid.js",
                       "~/Scripts/main.js"));

            BundleTable.Bundles.Add(new ScriptBundle("~/bundles/prettify").Include("~/Scripts/prettify.js"));
        }

    }
}