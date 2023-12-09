using System;
using System.Collections.Generic;
using System.Text;
using MovieMagnet.Localization;
using Volo.Abp.Application.Services;

namespace MovieMagnet.Services;

/* Inherit your application services from this class.
 */
public abstract class MovieMagnetAppService : ApplicationService
{
    protected MovieMagnetAppService()
    {
        LocalizationResource = typeof(MovieMagnetResource);
    }
}