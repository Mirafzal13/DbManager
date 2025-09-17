using DbManager.Application.UseCases.ConnectionConfigs.Models;

namespace DbManager.Application.UseCases.ConnectionConfigs.Mappers
{
    public class ConnectionConfigMappingProfile : Profile
    {
        public ConnectionConfigMappingProfile()
        {
            CreateMap<ConnectionConfig, ConnectionConfigModel>();
        }
    }
}
