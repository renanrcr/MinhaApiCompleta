using AutoMapper;
using DevIO.Api.ViewModels;
using DevIO.Business.Intefaces;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.Api.Controllers
{
    [Route("api/[controller]")]
    public class FornecedoresController : MaintController
    {
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IFornecedorService _fornecedorService;
        private readonly IEnderecoRepository _enderecoRepository;
        private readonly IMapper _mapper;

        public FornecedoresController(IFornecedorRepository fornecedorRepository,
                                        IFornecedorService fornecedorService,
                                        INotificador notificador,
                                        IMapper mapper,
                                        IEnderecoRepository enderecoRepository) : base(notificador)
        {
            _fornecedorRepository = fornecedorRepository;
            _fornecedorService = fornecedorService;
            _mapper = mapper;
            _enderecoRepository = enderecoRepository;
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos()
        {
            IEnumerable<FornecedorViewModel> fornecedores = _mapper.Map<IEnumerable<FornecedorViewModel>>(await _fornecedorRepository.ObterTodos());

            return StatusCode(StatusCodes.Status200OK, fornecedores);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            FornecedorViewModel fornecedores = await ObterFornecedorProdutoPorId(id);

            if (fornecedores == null) return NotFound();

            return StatusCode(StatusCodes.Status200OK, fornecedores);
        }

        [HttpPost]
        public async Task<IActionResult> Adicionar(FornecedorViewModel fornecedorViewModel)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            await _fornecedorService.Adicionar(_mapper.Map<Fornecedor>(fornecedorViewModel));

            return CustomResponse(fornecedorViewModel);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Atualizar(Guid id, FornecedorViewModel fornecedorViewModel)
        {
            if (id != fornecedorViewModel.Id)
            {
                NotificarErro("O id informado é diferente do que foi passado.");
                return CustomResponse();
            }

            if (!ModelState.IsValid) return CustomResponse(ModelState);

            await _fornecedorService.Atualizar(_mapper.Map<Fornecedor>(fornecedorViewModel));

            return Ok(fornecedorViewModel);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Excluir(Guid id)
        {
            var fornecedorViewModel = ObterFornecedorEndereco(id);

            if (fornecedorViewModel == null) return NotFound();

            await _fornecedorService.Remover(id);

            return CustomResponse();
        }

        [HttpGet("obter-endereco/{id:Guid}")]
        public async Task<IActionResult> ObterEnderecoPorId(Guid id)
        {
            EnderecoViewModel enderecoViewModel1 = _mapper.Map<EnderecoViewModel>(await _fornecedorRepository.ObterFornecedorProdutosEndereco(id));

            if (enderecoViewModel1 == null) return NotFound();

            return StatusCode(StatusCodes.Status200OK, enderecoViewModel1);
        }

        [HttpPut("atualizar-endereco{id:guid}")]
        public async Task<IActionResult> AtualizarEndereco(Guid id, EnderecoViewModel enderecoViewModel)
        {
            if (id != enderecoViewModel.Id)
            {
                NotificarErro("O id informado é diferente do que foi passado.");
                return CustomResponse();
            }

            if (!ModelState.IsValid) return CustomResponse(ModelState);

            await _enderecoRepository.Atualizar(_mapper.Map<Endereco>(enderecoViewModel));

            return Ok(enderecoViewModel);
        }

        private async Task<FornecedorViewModel> ObterFornecedorProdutoPorId(Guid id)
        {
            return _mapper.Map<FornecedorViewModel>(await _fornecedorRepository.ObterFornecedorProdutosEndereco(id));
        }

        private async Task<FornecedorViewModel> ObterFornecedorEndereco(Guid id)
        {
            return _mapper.Map<FornecedorViewModel>(await _fornecedorRepository.ObterFornecedorEndereco(id));
        }
    }
}