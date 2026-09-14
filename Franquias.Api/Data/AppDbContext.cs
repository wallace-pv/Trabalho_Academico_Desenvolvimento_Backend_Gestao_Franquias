using Microsoft.EntityFrameworkCore;
using Franquias.Api.Models;

namespace Franquias.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Franqueadora> Franqueadoras => Set<Franqueadora>();
    public DbSet<ResponsavelFranqueado> Responsaveis => Set<ResponsavelFranqueado>();
    public DbSet<UnidadeFranqueada> Unidades => Set<UnidadeFranqueada>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<CategoriaProduto> Categorias => Set<CategoriaProduto>();
    public DbSet<Fornecedor> Fornecedores => Set<Fornecedor>();
    public DbSet<ProdutoServico> Produtos => Set<ProdutoServico>();
    public DbSet<Estoque> Estoques => Set<Estoque>();
    public DbSet<MovimentacaoEstoque> MovimentacoesEstoque => Set<MovimentacaoEstoque>();
    public DbSet<Venda> Vendas => Set<Venda>();
    public DbSet<ItemVenda> ItensVenda => Set<ItemVenda>();
    public DbSet<Royalty> Royalties => Set<Royalty>();
    public DbSet<ChamadoSuporte> Chamados => Set<ChamadoSuporte>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Franqueadora
        modelBuilder.Entity<Franqueadora>(entity =>
        {
            entity.HasKey(f => f.Id);
            entity.HasIndex(f => f.CNPJ).IsUnique();
            entity.Property(f => f.RazaoSocial).HasMaxLength(150).IsRequired();
            entity.Property(f => f.NomeFantasia).HasMaxLength(100).IsRequired();
            entity.Property(f => f.CNPJ).HasMaxLength(18).IsRequired();
            entity.Property(f => f.PercentualPadraoRoyalty).HasPrecision(5, 2);
        });

        // ResponsavelFranqueado
        modelBuilder.Entity<ResponsavelFranqueado>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Nome).HasMaxLength(120).IsRequired();
            entity.Property(r => r.CPF).HasMaxLength(14).IsRequired();
            entity.Property(r => r.Email).HasMaxLength(120).IsRequired();
        });

        // UnidadeFranqueada
        modelBuilder.Entity<UnidadeFranqueada>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.CNPJ).IsUnique();
            entity.Property(u => u.Nome).HasMaxLength(120).IsRequired();
            entity.Property(u => u.CNPJ).HasMaxLength(18).IsRequired();
            entity.Property(u => u.CodigoUnidade).HasMaxLength(20).IsRequired();
            entity.Property(u => u.PercentualRoyalty).HasPrecision(5, 2);

            entity.HasOne(u => u.Franqueadora)
                  .WithMany(f => f.Unidades)
                  .HasForeignKey(u => u.FranqueadoraId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(u => u.Responsavel)
                  .WithMany(r => r.Unidades)
                  .HasForeignKey(u => u.ResponsavelId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Usuario
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Nome).HasMaxLength(120).IsRequired();
            entity.Property(u => u.Email).HasMaxLength(150).IsRequired();
            entity.Property(u => u.SenhaHash).IsRequired();

            entity.HasOne(u => u.Unidade)
                  .WithMany(u => u.Usuarios)
                  .HasForeignKey(u => u.UnidadeId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // CategoriaProduto
        modelBuilder.Entity<CategoriaProduto>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Nome).HasMaxLength(80).IsRequired();
        });

        // Fornecedor
        modelBuilder.Entity<Fornecedor>(entity =>
        {
            entity.HasKey(f => f.Id);
            entity.HasIndex(f => f.CNPJ).IsUnique();
            entity.Property(f => f.RazaoSocial).HasMaxLength(150).IsRequired();
            entity.Property(f => f.NomeFantasia).HasMaxLength(100).IsRequired();
            entity.Property(f => f.CNPJ).HasMaxLength(18).IsRequired();
        });

        // ProdutoServico
        modelBuilder.Entity<ProdutoServico>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.HasIndex(p => p.CodigoSKU).IsUnique();
            entity.Property(p => p.CodigoSKU).HasMaxLength(30).IsRequired();
            entity.Property(p => p.Nome).HasMaxLength(120).IsRequired();
            entity.Property(p => p.PrecoBase).HasPrecision(18, 2);

            entity.HasOne(p => p.Categoria)
                  .WithMany(c => c.Produtos)
                  .HasForeignKey(p => p.CategoriaId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(p => p.Fornecedor)
                  .WithMany(f => f.Produtos)
                  .HasForeignKey(p => p.FornecedorId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Estoque
        modelBuilder.Entity<Estoque>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UnidadeId, e.ProdutoId }).IsUnique();

            entity.HasOne(e => e.Unidade)
                  .WithMany(u => u.Estoques)
                  .HasForeignKey(e => e.UnidadeId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Produto)
                  .WithMany(p => p.Estoques)
                  .HasForeignKey(e => e.ProdutoId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // MovimentacaoEstoque
        modelBuilder.Entity<MovimentacaoEstoque>(entity =>
        {
            entity.HasKey(m => m.Id);

            entity.HasOne(m => m.Unidade)
                  .WithMany(u => u.MovimentacoesEstoque)
                  .HasForeignKey(m => m.UnidadeId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(m => m.Produto)
                  .WithMany(p => p.MovimentacoesEstoque)
                  .HasForeignKey(m => m.ProdutoId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(m => m.Usuario)
                  .WithMany()
                  .HasForeignKey(m => m.UsuarioId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Venda
        modelBuilder.Entity<Venda>(entity =>
        {
            entity.HasKey(v => v.Id);
            entity.HasIndex(v => v.CodigoVenda).IsUnique();
            entity.Property(v => v.CodigoVenda).HasMaxLength(30).IsRequired();
            entity.Property(v => v.ValorTotal).HasPrecision(18, 2);

            entity.HasOne(v => v.Unidade)
                  .WithMany(u => u.Vendas)
                  .HasForeignKey(v => v.UnidadeId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(v => v.Usuario)
                  .WithMany(u => u.Vendas)
                  .HasForeignKey(v => v.UsuarioId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ItemVenda
        modelBuilder.Entity<ItemVenda>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.PrecoUnitario).HasPrecision(18, 2);
            entity.Property(i => i.Subtotal).HasPrecision(18, 2);

            entity.HasOne(i => i.Venda)
                  .WithMany(v => v.Itens)
                  .HasForeignKey(i => i.VendaId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(i => i.Produto)
                  .WithMany(p => p.ItensVenda)
                  .HasForeignKey(i => i.ProdutoId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Royalty
        modelBuilder.Entity<Royalty>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.HasIndex(r => new { r.UnidadeId, r.MesReferencia, r.AnoReferencia }).IsUnique();
            entity.Property(r => r.FaturamentoBase).HasPrecision(18, 2);
            entity.Property(r => r.PercentualCobrado).HasPrecision(5, 2);
            entity.Property(r => r.ValorRoyalty).HasPrecision(18, 2);

            entity.HasOne(r => r.Unidade)
                  .WithMany(u => u.Royalties)
                  .HasForeignKey(r => r.UnidadeId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ChamadoSuporte
        modelBuilder.Entity<ChamadoSuporte>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Titulo).HasMaxLength(150).IsRequired();
            entity.Property(c => c.Descricao).IsRequired();

            entity.HasOne(c => c.Unidade)
                  .WithMany(u => u.Chamados)
                  .HasForeignKey(c => c.UnidadeId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(c => c.UsuarioAbertura)
                  .WithMany(u => u.ChamadosAbertos)
                  .HasForeignKey(c => c.UsuarioAberturaId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
