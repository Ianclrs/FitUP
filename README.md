# 🏋️‍♂️ FitUP

💥 **O FitUP é uma aplicação web completa de fitness e bem-estar projetada para ajudar usuários a gerenciarem seus planos de treino, guias de nutrição e evolução física de forma centralizada e intuitiva.**

Desenvolvido com uma arquitetura moderna e focado em alta performance, o projeto combina o poder do ecossistema .NET na sua versão mais recente com uma interface rica e responsiva.

---

## 🚀 Tecnologias Utilizadas

O projeto foi construído utilizando as melhores práticas de desenvolvimento de software e as tecnologias mais recentes do mercado:

- **Frontend:** [MudBlazor](https://mudblazor.com/) (Componentes UI para Blazor WebAssembly)
- **Backend:** [.NET 10](https://dotnet.microsoft.com/) & C# (ASP.NET Core Web API)
- **Persistência de Dados:** [Dapper ORM](https://github.com/DapperLib/Dapper) (Consultas de alta performance e mapeamento de dados)
- **Banco de Dados:** Microsoft SQL Server
- **Arquitetura:** Princípios de Clean Architecture / Domain-Driven Design (DDD) e modelagem relacional normalizada.

---

## ✨ Funcionalidades Principais

### 📋 Gerenciamento de Treinos
- Criação e customização de divisões de treino (Fullbody, PPL - Push/Pull/Legs, Upper/Lower, etc.).
- Registro detalhado de exercícios, séries, repetições.
- **Monte seu Treino** — ferramenta interativa para montar rotinas personalizadas.
- **Dicas de Treino** — orientações e boas práticas para diferentes objetivos.

### 🍎 Guias de Nutrição & Macronutrientes
- **Calculadora de Bioimpedância** — cálculo e análise de composição corporal (IMC, taxa metabólica basal, etc.).
- **Ganho Máximo** — cálculo e distribuição automatizada de macronutrientes (Proteínas, Carboidratos e Gorduras) com base no objetivo (Hipertrofia, Definição, Manutenção).
- Organização de diários alimentares ou planos estruturados.

### 👤 Gestão de Usuários & Perfil
- Cadastro e autenticação de usuários (**Login** / **Cadastro**).
- Painel de controle (**Home**) interativo centralizado para visualização rápida do progresso e rotina diária.
- **Mensagens Customizadas** para feedback e motivação personalizada.

### 📄 Páginas Institucionais
- **Termos de Uso** e **Política de Privacidade**.
- **Indicações** e recomendações de uso da plataforma.

---

## 🛠️ Como Executar o Projeto Localmente

### Pré-requisitos
Antes de começar, você vai precisar ter instalado em sua máquina:
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server](https://www.microsoft.com/sql-server/) (ou LocalDB)
- IDE de sua preferência (Visual Studio 2022, VS Code ou Rider)

### Passos para Configuração

1. **Clonar o Repositório:**
   ```bash
   git clone https://github.com/Ianclrs/FitUP.git
   cd FitUP
   ```

2. **Restaurar as dependências:**
   ```bash
   dotnet restore
   ```

3. **Buildar o projeto:**
   ```bash
   dotnet build
   ```

4. **Executar o Frontend (Blazor WebAssembly):**
   ```bash
   cd FitUP/FitUP
   dotnet run
   ```
   O frontend estará disponível em `https://localhost:5001` ou `http://localhost:5000`.

5. **Executar a Web API (Backend):**
   Em um terminal separado:
   ```bash
   cd src/FitUP.WebApi
   dotnet run
   ```
   A API estará disponível em `https://localhost:7001` ou `http://localhost:7000`.

6. **Configurar o Banco de Dados:**
   - Atualize a string de conexão no arquivo de configuração da Web API (`appsettings.json`) com os dados do seu SQL Server.
   - Execute os scripts de migração (se disponíveis) para criar as tabelas necessárias.

---

## 📝 Colaboradores

<table>
  <tr>
    <td align="center">
    <a>
    <img src="https://avatars.githubusercontent.com/u/210450237?v=4"
             width="150"
             height="150"
             style="object-fit: cover; border-radius: 50%;"
             alt=""/><br>
          <sub><b>Ian Carlos como DEV</b></sub>
      </a>
     </td>
    <td align="center">
      <a>
      <img src="https://avatars.githubusercontent.com/u/161505109?v=4"
            width="150"
            height="150"
            style="object-fit: cover; border-radius: 50%;"
            alt=""/><br>
          <sub><b>Thauã Cerqueira como DEV</b></sub>
      </a>
    </td>
    <td align="center">
      <a>
      <img src="https://avatars.githubusercontent.com/u/146496842?v=4"
          width="150"
          height="150"
          style="object-fit: fill; border-radius: 50%;"
          alt=""/><br>
          <sub><b>Felipe Dario como SM</b></sub>      
     </a>
    </td>
    <td align="center">
    <a>
    <img src="https://avatars.githubusercontent.com/u/203004835?v=4"
             width="150"
             height="150"
             style="object-fit: cover; border-radius: 50%;"
             alt=""/><br>
          <sub><b>Pedro Freitas como QA </b></sub>
      </a>
      <td align="center">
    <a>
    <img src="https://avatars.githubusercontent.com/u/247213111?v=4"
             width="150"
             height="150"
             style="object-fit: cover; border-radius: 50%;"
             alt=""/><br>
          <sub><b>Pedro Henrique como PO</b></sub>
      </a>
        <td align="center">
    <a>
    <img src="https://avatars.githubusercontent.com/u/213353198?v=4"
             width="150"
             height="150"
             style="object-fit: cover; border-radius: 50%;"
             alt=""/><br>
          <sub><b>Gabriel Castor como QA</b></sub>
      </a>
     </td>
  </tr>  
</table>