using AuthService.Models;

namespace AuthServiceTest.Domain.Entidades
{
    [TestClass]
    public class UsuarioTest
    {
        [TestMethod]
        public void TestarGetSetPropriedades()
        {
            var usuario = new Usuario
            {
                Id = 1,
                Nome = "Teste",
                Email = "teste@mail.com",
                Senha = "teste123",
                TipoUsuario = TipoUsuarioEnum.Administrador
            };

            Assert.AreEqual(1, usuario.Id);
            Assert.AreEqual("Teste", usuario.Nome);
            Assert.AreEqual("teste@mail.com", usuario.Email);
            Assert.AreEqual("teste123", usuario.Senha);
            Assert.AreEqual(TipoUsuarioEnum.Administrador, usuario.TipoUsuario);
        }
    }
}