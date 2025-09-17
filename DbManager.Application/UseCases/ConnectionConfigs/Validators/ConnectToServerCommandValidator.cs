using DbManager.Application.UseCases.ConnectionConfigs.Commands;

namespace DbManager.Application.UseCases.ConnectionConfigs.Validators
{
    public class ConnectToServerCommandValidator : AbstractValidator<ConnectToServerCommand>
    {
        public ConnectToServerCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Host).NotEmpty();
            RuleFor(x => x.Port).NotEmpty();
            RuleFor(x => x.Username).NotEmpty();
        }
    }
}
