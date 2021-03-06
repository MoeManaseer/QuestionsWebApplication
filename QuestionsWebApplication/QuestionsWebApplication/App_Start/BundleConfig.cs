using System.Web;
using System.Web.Optimization;

namespace QuestionsWebApplication
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/QuestionsJS").Include(
                      "~/Scripts/QuestionsContainersHandler.js"));

            bundles.Add(new ScriptBundle("~/bundles/QuestionsIndexControllers").Include(
                      "~/Scripts/jquery.unobtrusive*",
                      "~/Scripts/ApplicationShared.js",
                      "~/Scripts/QuestionsSharedHandlers.js",
                      "~/Scripts/QuestionsSortHandler.js",
                      "~/Scripts/QuestionsRefreshHandler.js",
                      "~/Scripts/QuestionsDeleteHandler.js"));

            bundles.Add(new ScriptBundle("~/bundles/Settings").Include(
                "~/Scripts/ApplicationShared.js",
                "~/Scripts/SettingsHandler.js",
                "~/Scripts/SettingsConnectionTestHandler.js"
                ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/QuestionsStyles.css",
                      "~/Content/ArabicStyles.css",
                      "~/Content/ErrorPageStyles.css"));
        }
    }
}
