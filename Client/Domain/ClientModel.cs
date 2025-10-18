using System;
using System.ComponentModel.DataAnnotations;

namespace Client.Model
{
    public class ClientModel
    {
        // Identificador único do cliente
        public Guid Id { get; set; } = Guid.NewGuid();

        // Nome do cliente
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome não pode ter mais de 100 caracteres")]
        public string Name { get; set; }

        // Sobrenome do cliente
        [Required(ErrorMessage = "O sobrenome é obrigatório")]
        [StringLength(100, ErrorMessage = "O sobrenome não pode ter mais de 100 caracteres")]
        public string Surname { get; set; }

        // Email de contato
        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(150, ErrorMessage = "O email não pode ter mais de 150 caracteres")]
        public string Email { get; set; }

        // Data de nascimento
        [Required(ErrorMessage = "A data de nascimento é obrigatória")]
        public DateTime Birthdate { get; set; }

        // Indicador se o cliente está ativo
        public bool IsActive { get; set; } = true;

        // Data de criação do registro
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Data de atualização do registro
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
