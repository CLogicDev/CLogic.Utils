using CLogic.Utils.Shared;
namespace CLogic.Utils.Tests
{
    public class ExampleUserPreferences : PreferencesSo<ExampleUserPreferences>
    {
        protected override string FileName { get; } = "ExampleUserPreferences.asset";

        public override string MenuPath => "Preferences/CLogic/Example";

        public bool value;

        public string text;
    }
}
