using System;
using Volo.Abp.Application.Dtos;

namespace MovieMagnet.Services.Dtos;

public class GenresDto : EntityDto<long>
{
    public string Name { get; set; }
}

