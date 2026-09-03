using ERP.Application.DTOs.Mailing;
using ERP.Domain.Enums;
using FluentValidation;

namespace ERP.Application.Validators;

public class MailingCreateValidator : AbstractModelValidator<MailingCreateDto>
{
    public MailingCreateValidator()
    {
        RuleFor(x => x.TipoPessoa)
            .IsInEnum().WithMessage("Tipo de Pessoa é obrigatório.");

        When(x => x.TipoPessoa == TipoPessoa.Juridica, () =>
        {
            RuleFor(x => x.RazaoSocial)
                .NotEmpty().WithMessage("Razão Social é obrigatória para Pessoa Jurídica.")
                .MaximumLength(150).WithMessage("Razão Social deve ter no máximo 150 caracteres.");
        });

        When(x => x.TipoPessoa == TipoPessoa.Fisica, () =>
        {
            RuleFor(x => x.NomeCompleto)
                .NotEmpty().WithMessage("Nome Completo é obrigatório para Pessoa Física.")
                .MaximumLength(150).WithMessage("Nome Completo deve ter no máximo 150 caracteres.");
        });

        RuleFor(x => x.IndIe)
            .IsInEnum().WithMessage("Indicador da Inscrição Estadual é obrigatório.");

        RuleForEach(x => x.Enderecos).ChildRules(endereco =>
        {
            endereco.RuleFor(e => e.Cep).NotEmpty().WithMessage("CEP do endereço é obrigatório.");
            endereco.RuleFor(e => e.Logradouro).NotEmpty().WithMessage("Logradouro é obrigatório.");
            endereco.RuleFor(e => e.Numero).NotEmpty().WithMessage("Número do endereço é obrigatório.");
            endereco.RuleFor(e => e.Bairro).NotEmpty().WithMessage("Bairro é obrigatório.");
            endereco.RuleFor(e => e.Cidade).NotEmpty().WithMessage("Cidade é obrigatória.");
            endereco.RuleFor(e => e.Estado).NotEmpty().WithMessage("UF/Estado é obrigatório.");
        });

        RuleForEach(x => x.Contatos).ChildRules(contato =>
        {
            contato.RuleFor(c => c.Nome).NotEmpty().WithMessage("Nome do contato é obrigatório.");
            contato.When(c => !string.IsNullOrEmpty(c.Email), () =>
            {
                contato.RuleFor(c => c.Email).EmailAddress().WithMessage("Email do contato é inválido.");
            });
        });
    }
}

public class MailingUpdateValidator : AbstractModelValidator<MailingUpdateDto>
{
    public MailingUpdateValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Id inválido.");
        Include(new MailingCreateValidator());
    }
}

public abstract class AbstractModelValidator<T> : AbstractValidator<T>
{
}
