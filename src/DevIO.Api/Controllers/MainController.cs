using DevIO.Business.Intefaces;
using DevIO.Business.Notificacoes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DevIO.Api.Controllers
{
    [ApiController]
    public abstract class MaintController : ControllerBase
    {
        private readonly INotificador _notificador;

        public MaintController(INotificador notificador)
        {
            _notificador = notificador;
        }

        protected bool OperacaoValida()
        {
            return !_notificador.TemNotificacao();
        }

        protected ActionResult CustomResponse(object result = null)
        {
            if (OperacaoValida())
                return Ok(new
                {
                    success = true,
                    data = result,
                });

            return BadRequest(new
            {
                success = false,
                errors = _notificador.ObterNotificacoes().Select(x => x.Mensagem),
            });
        }

        protected ActionResult CustomResponse(ModelStateDictionary modelState)
        {
            if (!modelState.IsValid) NotificarErroModelInvalida(modelState);

            return CustomResponse();
        }

        protected void NotificarErroModelInvalida(ModelStateDictionary modelState)
        {
            modelState.Values.SelectMany(x => x.Errors).ToList().ForEach(x =>
            {
                var erros = x.Exception == null ? x.ErrorMessage : x.Exception.Message;
                NotificarErro(erros);
            });
        }

        protected void NotificarErro(string mensagem)
        {
            _notificador.Handle(new Notificacao(mensagem));
        }
    }
}