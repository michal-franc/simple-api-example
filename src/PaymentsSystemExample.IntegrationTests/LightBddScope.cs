using LightBDD.Core.Configuration;
using LightBDD.XUnit2;

[assembly: ConfiguredLightBddScope]

// This is class is required for LightBDD runner configuration
internal class ConfiguredLightBddScopeAttribute : LightBddScopeAttribute
{
    protected override void OnConfigure(LightBddConfiguration configuration)
    {
    }

    protected override void OnSetUp()
    {
    }

    protected override void OnTearDown()
    {
    }
}