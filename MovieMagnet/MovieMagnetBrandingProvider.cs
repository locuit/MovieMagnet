using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace MovieMagnet;

[Dependency(ReplaceServices = true)]
public class MovieMagnetBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "MovieMagnet";
}
