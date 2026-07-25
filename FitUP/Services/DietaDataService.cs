namespace FitUP.Services;

public class DietaAlimento
{
    public string Label { get; set; } = string.Empty;
    public string Valor { get; set; } = string.Empty;
}

public class DietaRefeicao
{
    public string Titulo { get; set; } = string.Empty;
    public string? Imagem { get; set; }
    public List<DietaAlimento> Alimentos { get; set; } = new();
}

public class DietaCompleta
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public List<DietaRefeicao> Refeicoes { get; set; } = new();
}

public class DietaDataService
{
    public IReadOnlyList<DietaCompleta> Dietas { get; }

    public DietaDataService()
    {
        Dietas = GetDietas();
    }

    public DietaCompleta? GetById(int id) => Dietas.FirstOrDefault(d => d.Id == id);

    private static List<DietaCompleta> GetDietas()
    {
        return new()
        {
            new()
            {
                Id = 1,
                Nome = "Bulking Limpo",
                Descricao = "Foco no superávit calórico controlado. O aporte de carboidratos é alto para fornecer energia para treinos intensos e sinalizar a via mTOR (crescimento muscular).",
                Refeicoes = new()
                {
                    new()
                    {
                        Titulo = "Café da Manhã",
                        Imagem = "img-gm/cafe1.png",
                        Alimentos = new()
                        {
                            new() { Label = "Proteína: ", Valor = "Ovos mexidos ou Whey Protein." },
                            new() { Label = "Carboidrato: ", Valor = "Aveia em flocos." },
                            new() { Label = "Fruta: ", Valor = "Banana picada." },
                            new() { Label = "Gordura: ", Valor = "A gordura natural da gema ou 1 colher de pasta de amendoim." }
                        }
                    },
                    new()
                    {
                        Titulo = "Almoço",
                        Imagem = "img-gm/almoco1.png",
                        Alimentos = new()
                        {
                            new() { Label = "Proteína: ", Valor = "150g-200g de Peito de Frango ou Patinho." },
                            new() { Label = "Carboidrato: ", Valor = "200g-250g de Arroz Branco ou Batata." },
                            new() { Label = "Vegetais: ", Valor = "Grande porção de Brócolis e Cenoura no vapor." },
                            new() { Label = "Gordura: ", Valor = "1 colher de sopa de Azeite de Oliva Extra Virgem." }
                        }
                    },
                    new()
                    {
                        Titulo = "Pré-Treino",
                        Imagem = "img-gm/pretrei1.png",
                        Alimentos = new()
                        {
                            new() { Label = "Carboidrato: ", Valor = "200g de Batata Doce ou Macarrão integral." },
                            new() { Label = "Proteína: ", Valor = "100g de Tilápia ou Frango." },
                            new() { Label = "Fruta: ", Valor = "1 rodela de abacaxi (ajuda na digestão para não treinar pesado com o estômago cheio)." }
                        }
                    },
                    new()
                    {
                        Titulo = "Pós-Treino",
                        Imagem = "img-gm/pos1.png",
                        Alimentos = new()
                        {
                            new() { Label = "Carboidrato: ", Valor = "Suco de uva integral ou Maltodextrina." },
                            new() { Label = "Proteína: ", Valor = "Whey Protein Isolado." }
                        }
                    },
                    new()
                    {
                        Titulo = "Janta",
                        Imagem = "img-gm/janta1.png",
                        Alimentos = new()
                        {
                            new() { Label = "Carboidrato: ", Valor = "150g de Mandioca ou Arroz." },
                            new() { Label = "Proteína: ", Valor = "150g de Salmão ou Carne Vermelha magra." },
                            new() { Label = "Vegetais: ", Valor = "Salada de folhas verdes escuras (Espinafre/Rúcula)." }
                        }
                    },
                    new()
                    {
                        Titulo = "Ceia",
                        Imagem = "img-gm/ceia1.png",
                        Alimentos = new()
                        {
                            new() { Label = "Gordura: ", Valor = "Um punhado de Castanhas do Pará ou Abacate." },
                            new() { Label = "Proteína: ", Valor = "Iogurte Grego ou Albumina." },
                            new() { Label = "Fruta: ", Valor = "Morangos (baixo IG)." }
                        }
                    }
                }
            },
            new()
            {
                Id = 2,
                Nome = "Cutting / Secar",
                Descricao = "O objetivo aqui é o déficit calórico preservando a massa magra. Aumentamos a ingestão de proteínas para manter o balanço nitrogenado positivo e utilizamos carboidratos complexos de baixo índice glicêmico para controle da saciedade.",
                Refeicoes = new()
                {
                    new()
                    {
                        Titulo = "Café da Manhã",
                        Imagem = "img-gm/cafe2.png",
                        Alimentos = new()
                        {
                            new() { Label = "Vegetais: ", Valor = "Espinafre à vontade." },
                            new() { Label = "Proteína: ", Valor = "Omelete (4 ovos inteiros)." },
                            new() { Label = "Carboidrato: ", Valor = "30g de aveia em flocos." }
                        }
                    },
                    new()
                    {
                        Titulo = "Almoço",
                        Imagem = "img-gm/almoco2.png",
                        Alimentos = new()
                        {
                            new() { Label = "Carboidrato: ", Valor = "150g de batata-doce." },
                            new() { Label = "Proteína: ", Valor = "200g de peito de frango grelhado." },
                            new() { Label = "Vegetais: ", Valor = "Brócolis à vontade." }
                        }
                    },
                    new()
                    {
                        Titulo = "Lanche da Tarde",
                        Imagem = "img-gm/lanch2.png",
                        Alimentos = new()
                        {
                            new() { Label = "Gordura: ", Valor = "20g de castanhas do pará." },
                            new() { Label = "Proteína: ", Valor = "1 dose de Whey Protein." }
                        }
                    },
                    new()
                    {
                        Titulo = "Janta",
                        Imagem = "img-gm/janta2.png",
                        Alimentos = new()
                        {
                            new() { Label = "Vegetais: ", Valor = "Salada verde variada com 1 colher de sopa de azeite de oliva extra virgem." },
                            new() { Label = "Proteína: ", Valor = "200g de filé de tilápia ou pescada." }
                        }
                    }
                }
            },
            new()
            {
                Id = 3,
                Nome = "Bulking Limpo",
                Descricao = "Outra dieta também voltada superávit calórico controlado. Com bastante carboidrato para fornecer energia para treinos intensos e sinalizar a via mTOR (crescimento muscular).",
                Refeicoes = new()
                {
                    new()
                    {
                        Titulo = "Café da Manhã",
                        Imagem = "img-gm/cafe3.png",
                        Alimentos = new()
                        {
                            new() { Label = "Carboidratos: ", Valor = "400ml de leite integral e 60g de aveia." },
                            new() { Label = "Frutas: ", Valor = "2 bananas." },
                            new() { Label = "Proteína: ", Valor = "1 dose de Whey." },
                            new() { Label = "Gorduras: ", Valor = "1 colher de pasta de amendoim." }
                        }
                    },
                    new()
                    {
                        Titulo = "Almoço",
                        Imagem = "img-gm/almoco3.png",
                        Alimentos = new()
                        {
                            new() { Label = "Carboidratos: ", Valor = "300g de arroz branco e 100g de feijão." },
                            new() { Label = "Proteína: ", Valor = "200g de patinho moído ou bife magro." }
                        }
                    },
                    new()
                    {
                        Titulo = "Lanche da Tarde",
                        Imagem = "img-gm/lanch3.png",
                        Alimentos = new()
                        {
                            new() { Label = "Carboidratos: ", Valor = "2 fatias de pão de forma integral." },
                            new() { Label = "Proteína: ", Valor = "1 latinha de atum." },
                            new() { Label = "Gorduras: ", Valor = "Creme de ricota light." },
                            new() { Label = "Frutas: ", Valor = "1 Maçã." }
                        }
                    },
                    new()
                    {
                        Titulo = "Jantar",
                        Imagem = "img-gm/janta3.png",
                        Alimentos = new()
                        {
                            new() { Label = "Carboidratos: ", Valor = "250g de macarrão integral com molho de tomate natural." },
                            new() { Label = "Proteína: ", Valor = "200g de sobrecoxa de frango (sem pele)." }
                        }
                    },
                    new()
                    {
                        Titulo = "Ceia",
                        Imagem = "img-gm/ceia3.png",
                        Alimentos = new()
                        {
                            new() { Label = "Gorduras: ", Valor = "1 iogurte grego natural com 30g de mix de sementes (girassol/abóbora)." }
                        }
                    }
                }
            },
            new()
            {
                Id = 4,
                Nome = "Carb Cycling",
                Descricao = "Esta estratégia alterna dias de alto carboidrato (em treinos pesados de pernas ou costas) com dias de baixo carboidrato. Uma excelente variação por ser excelente para quem quer ganhar massa muscular com o mínimo de ganho de gordura.",
                Refeicoes = new()
                {
                    new()
                    {
                        Titulo = "Café da Manhã",
                        Imagem = "img-gm/cafe4.png",
                        Alimentos = new()
                        {
                            new() { Label = "Carboidratos: ", Valor = "2 fatias de pão de forma integral com sementes." },
                            new() { Label = "Proteína: ", Valor = "4 ovos mexidos." }
                        }
                    },
                    new()
                    {
                        Titulo = "Almoço",
                        Imagem = "img-gm/almoco4.png",
                        Alimentos = new()
                        {
                            new() { Label = "Carboidratos: ", Valor = "200g de mandioca cozida." },
                            new() { Label = "Proteína: ", Valor = "200g de sobrecoxa de frango grelhado." },
                            new() { Label = "Vegetais: ", Valor = "Salada de folhas." }
                        }
                    },
                    new()
                    {
                        Titulo = "Pré-Treino",
                        Imagem = "img-gm/pretrei4.png",
                        Alimentos = new()
                        {
                            new() { Label = "Carboidratos: ", Valor = "30g de aveia." },
                            new() { Label = "Frutas: ", Valor = "1 Banana." }
                        }
                    },
                    new()
                    {
                        Titulo = "Jantar",
                        Imagem = "img-gm/janta4.png",
                        Alimentos = new()
                        {
                            new() { Label = "Carboidratos: ", Valor = "150g de arroz." },
                            new() { Label = "Vegetais: ", Valor = "Legumes salteados na manteiga." },
                            new() { Label = "Proteína: ", Valor = "200g de carne bovina magra (alcatra/mignon)." }
                        }
                    },
                    new()
                    {
                        Titulo = "Ceia",
                        Imagem = "img-gm/ceia4.png",
                        Alimentos = new()
                        {
                            new() { Label = "Proteínas: ", Valor = "30g de albumina ou caseína." }
                        }
                    }
                }
            }
        };
    }
}