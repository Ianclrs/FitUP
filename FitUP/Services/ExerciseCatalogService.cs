using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace FitUP.Services;

public class ExerciseItem
{
    public string Name { get; set; } = string.Empty;
    public int Series { get; set; }
    public string Reps { get; set; } = string.Empty;
    public string Rest { get; set; } = string.Empty;
    public string Muscle { get; set; } = string.Empty;
}

public class WorkoutExercise
{
    public string Name { get; set; } = string.Empty;
    public string Muscle { get; set; } = string.Empty;
    public int Series { get; set; }
    public string Reps { get; set; } = string.Empty;
    public string Rest { get; set; } = string.Empty;
}

public class WorkoutSplitData
{
    public string Label { get; set; } = string.Empty;
    public string Focus { get; set; } = string.Empty;
    public List<WorkoutExercise> Exercises { get; set; } = new();
}

public class WorkoutPlanData
{
    public string Nome { get; set; } = string.Empty;
    public string Desc { get; set; } = string.Empty;
    public List<WorkoutSplitData> Splits { get; set; } = new();
}

public class WorkoutSplit
{
    public string Label { get; set; } = string.Empty;
    public string Focus { get; set; } = string.Empty;
    public List<ExerciseItem> Exercises { get; set; } = new();
}

public class WorkoutPlan
{
    public string Nome { get; set; } = string.Empty;
    public string Desc { get; set; } = string.Empty;
    public List<WorkoutSplit> Splits { get; set; } = new();
}

public class ExerciseCatalogService
{
    private readonly HttpClient _httpClient;

    public IReadOnlyDictionary<string, ExerciseItem> Catalog { get; private set; } = new Dictionary<string, ExerciseItem>();
    public IReadOnlyDictionary<string, string[]> FocusMapping { get; private set; } = new Dictionary<string, string[]>();
    public IReadOnlyDictionary<string, string[]> LimitacaoBloqueios { get; private set; } = new Dictionary<string, string[]>();
    public IReadOnlyDictionary<string, string> ExerciseNameToKey { get; private set; } = new Dictionary<string, string>();
    public IReadOnlyDictionary<string, WorkoutPlan> WorkoutTemplates { get; private set; } = new Dictionary<string, WorkoutPlan>();

    public ExerciseCatalogService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task InitializeAsync()
    {
        await LoadExerciseCatalogAsync();
        await LoadFocusMappingsAsync();
        await LoadWorkoutTemplatesAsync();
        BuildReverseLookup();
        InitializeLimitacaoBloqueios();
    }

    private async Task LoadExerciseCatalogAsync()
    {
        try
        {
            var json = await _httpClient.GetStringAsync("data/exercises.json");
            var items = JsonSerializer.Deserialize<List<JsonExerciseItem>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (items != null && items.Count > 0)
            {
                Catalog = items.ToDictionary(
                    i => i.Key,
                    i => new ExerciseItem { Name = i.Name, Series = i.Series, Reps = i.Reps, Rest = i.Rest, Muscle = i.Muscle }
                );
                return;
            }
        }
        catch { /* fallback to hardcoded */ }
        Catalog = GetHardcodedCatalog();
    }

    private async Task LoadFocusMappingsAsync()
    {
        try
        {
            var json = await _httpClient.GetStringAsync("data/focus-mappings.json");
            var mappings = JsonSerializer.Deserialize<Dictionary<string, string[]>>(json);
            if (mappings != null && mappings.Count > 0)
            {
                FocusMapping = mappings;
                return;
            }
        }
        catch { /* fallback to hardcoded */ }
        FocusMapping = GetHardcodedFocusMapping();
    }

    private async Task LoadWorkoutTemplatesAsync()
    {
        try
        {
            var json = await _httpClient.GetStringAsync("data/workout-templates.json");
            var templates = JsonSerializer.Deserialize<Dictionary<string, WorkoutPlanData>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (templates != null && templates.Count > 0)
            {
                WorkoutTemplates = templates.ToDictionary(
                    kvp => kvp.Key,
                    kvp => new WorkoutPlan
                    {
                        Nome = kvp.Value.Nome,
                        Desc = kvp.Value.Desc,
                        Splits = kvp.Value.Splits.Select(s => new WorkoutSplit
                        {
                            Label = s.Label,
                            Focus = s.Focus,
                            Exercises = s.Exercises.Select(e => new ExerciseItem
                            {
                                Name = e.Name,
                                Muscle = e.Muscle,
                                Series = e.Series,
                                Reps = e.Reps,
                                Rest = e.Rest
                            }).ToList()
                        }).ToList()
                    }
                );
                return;
            }
        }
        catch { /* fallback to hardcoded */ }
        WorkoutTemplates = GetHardcodedWorkoutTemplates();
    }

    private void BuildReverseLookup()
    {
        var reverse = new Dictionary<string, string>();
        foreach (var kvp in Catalog)
        {
            if (!reverse.ContainsKey(kvp.Value.Name))
                reverse[kvp.Value.Name] = kvp.Key;
        }
        ExerciseNameToKey = reverse;
    }

    private void InitializeLimitacaoBloqueios()
    {
        LimitacaoBloqueios = new Dictionary<string, string[]>
        {
            ["Joelho"] = new[] { "agachamento", "agachamento-sumo", "agachamento-bulgaro", "agachamento-goblet", "agachamento-hack", "agachamento-smith", "leg-press", "afundo", "agachamento-salto", "burpee" },
            ["Coluna"] = new[] { "terra", "stiff", "remada-curvada", "agachamento", "agachamento-sumo", "agachamento-smith", "remada-cavalinho", "back-extension" },
            ["Ombro"] = new[] { "desenvolvimento", "elevacao-lateral", "elevacao-frontal", "arnold-press", "remada-alta", "supino-reto", "supino-inclinado", "supino-declinado", "flexao" },
            ["Cotovelo"] = new[] { "rosca-direta", "rosca-simultanea", "rosca-alternada", "rosca-martelo", "rosca-scott", "rosca-concentrada", "rosca-inclinada", "rosca-spider", "rosca-direta-polia", "triceps-frances", "triceps-testa", "triceps-corda", "triceps-pulley", "triceps-coice", "flexao-fechada", "mergulho" },
            ["Punho"] = new[] { "supino-reto", "supino-inclinado", "supino-declinado", "desenvolvimento", "flexao", "flexao-fechada", "puxada-alta", "mergulho", "burpee", "mountain-climber" },
            ["Quadril"] = new[] { "agachamento-sumo", "agachamento-bulgaro", "stiff", "elevacao-pelvica", "afundo", "leg-press", "cadeira-extensora", "cadeira-flexora", "agachamento-salto" },
            ["Tornozelo"] = new[] { "agachamento-salto", "burpee", "pulo-corda", "corrida-intervalada", "jumping-jacks", "mountain-climber", "afundo", "agachamento-bulgaro" },
        };
    }

    public Dictionary<string, ExerciseItem> GetFilteredCatalog(string focus)
    {
        if (FocusMapping.TryGetValue(focus, out var keys))
            return Catalog.Where(kvp => keys.Contains(kvp.Key)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        return Catalog.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    public bool TryGetKeyByName(string exerciseName, out string key)
    {
        return ExerciseNameToKey.TryGetValue(exerciseName, out key!);
    }

    // --- Hardcoded fallbacks ---

    private static Dictionary<string, ExerciseItem> GetHardcodedCatalog()
    {
        return new()
        {
            ["supino-reto"] = new() { Name = "Supino Reto Máquina ou Barra", Series = 4, Reps = "8–12", Rest = "60s", Muscle = "Peitoral maior" },
            ["supino-inclinado"] = new() { Name = "Supino Inclinado Máquina ou Barra", Series = 4, Reps = "8–12", Rest = "60s", Muscle = "Peitoral superior" },
            ["crucifixo-inclinado"] = new() { Name = "Crucifixo Inclinado Máquina ou Halter", Series = 3, Reps = "8–12", Rest = "60s", Muscle = "Peitoral superior" },
            ["crucifixo-reto"] = new() { Name = "Crucifixo Reto", Series = 3, Reps = "8–12", Rest = "60s", Muscle = "Peitoral maior" },
            ["voador-frontal"] = new() { Name = "Voador Frontal", Series = 3, Reps = "8–10", Rest = "60s", Muscle = "Peitoral maior" },
            ["crossover-polia-alta"] = new() { Name = "Cross Over Polia Alta", Series = 3, Reps = "8–12", Rest = "60s", Muscle = "Peitoral superior" },
            ["crossover-polia-baixa"] = new() { Name = "Cross Over Polia Baixa", Series = 3, Reps = "8–12", Rest = "60s", Muscle = "Peitoral inferior" },
            ["crossover-polia-media"] = new() { Name = "Cross Over Polia Média", Series = 3, Reps = "8–12", Rest = "60s", Muscle = "Peitoral maior" },
            ["flexao"] = new() { Name = "Flexão de Braços", Series = 3, Reps = "10–20", Rest = "45s", Muscle = "Peito & Ombros" },
            ["supino-declinado"] = new() { Name = "Supino Declinado", Series = 4, Reps = "8–12", Rest = "90s", Muscle = "Peitoral inferior" },
            ["puxada-frontal"] = new() { Name = "Puxada Frontal", Series = 4, Reps = "8–12", Rest = "60s", Muscle = "Costas" },
            ["remada-curvada"] = new() { Name = "Remada Curvada com Barra", Series = 4, Reps = "8–12", Rest = "75s", Muscle = "Costas & Lombar" },
            ["remada-cavalinho"] = new() { Name = "Remada Cavalinho", Series = 3, Reps = "5–6", Rest = "120s", Muscle = "Costas" },
            ["remada-unilateral"] = new() { Name = "Remada Unilateral", Series = 3, Reps = "8–10", Rest = "90s", Muscle = "Costas" },
            ["pulldown"] = new() { Name = "Pulldown Polia", Series = 3, Reps = "8–10", Rest = "120s", Muscle = "Costas" },
            ["crucifixo-inverso"] = new() { Name = "Crucifixo Inverso", Series = 3, Reps = "8–10", Rest = "90s", Muscle = "Costas & Ombros" },
            ["puxada-alta"] = new() { Name = "Puxada Alta no Pulley", Series = 4, Reps = "6–10", Rest = "90s", Muscle = "Latíssimo do dorso" },
            ["desenvolvimento"] = new() { Name = "Desenvolvimento Máquina ou Halter", Series = 3, Reps = "8–12", Rest = "60s", Muscle = "Deltoides" },
            ["elevacao-lateral"] = new() { Name = "Elevação Lateral Polia ou Halter", Series = 3, Reps = "8–12", Rest = "60s", Muscle = "Deltoides medial e posterior" },
            ["elevacao-frontal"] = new() { Name = "Elevação Frontal", Series = 3, Reps = "8–12", Rest = "60s", Muscle = "Deltoides frontal" },
            ["arnold-press"] = new() { Name = "Arnold Press", Series = 3, Reps = "10–12", Rest = "90s", Muscle = "Deltoides" },
            ["remada-alta"] = new() { Name = "Remada Alta", Series = 4, Reps = "8–10", Rest = "60s", Muscle = "Deltoides" },
            ["face-pull"] = new() { Name = "Face Pull", Series = 4, Reps = "8–10", Rest = "90s", Muscle = "Deltoides" },
            ["triceps-frances"] = new() { Name = "Tríceps Francês Corda ou Halter", Series = 3, Reps = "10–12", Rest = "60s", Muscle = "Tríceps" },
            ["triceps-testa"] = new() { Name = "Tríceps Testa Corda ou Barra W", Series = 3, Reps = "10–12", Rest = "60s", Muscle = "Tríceps" },
            ["triceps-corda"] = new() { Name = "Tríceps Corda na Polia", Series = 4, Reps = "8-12", Rest = "60s", Muscle = "Tríceps" },
            ["triceps-pulley"] = new() { Name = "Tríceps Polia Alta", Series = 4, Reps = "10–12", Rest = "60s", Muscle = "Tríceps" },
            ["triceps-coice"] = new() { Name = "Tríceps Coice", Series = 3, Reps = "10–12", Rest = "60s", Muscle = "Tríceps" },
            ["flexao-fechada"] = new() { Name = "Flexão de Braços Pegada Fechada", Series = 3, Reps = "10–20", Rest = "60s", Muscle = "Tríceps & Peito" },
            ["mergulho"] = new() { Name = "Mergulho entre Bancos", Series = 3, Reps = "10–12", Rest = "60s", Muscle = "Tríceps & Peito" },
            ["rosca-direta"] = new() { Name = "Rosca Direta com Barra", Series = 3, Reps = "6–8", Rest = "60s", Muscle = "Bíceps" },
            ["rosca-simultanea"] = new() { Name = "Rosca Simultânea com Barra ou Halter", Series = 3, Reps = "10–12", Rest = "60s", Muscle = "Bíceps" },
            ["rosca-direta-polia"] = new() { Name = "Rosca Direta na Polia", Series = 3, Reps = "6–8", Rest = "60s", Muscle = "Bíceps" },
            ["rosca-concentrada"] = new() { Name = "Rosca Concentrada", Series = 3, Reps = "10–12", Rest = "60s", Muscle = "Bíceps" },
            ["rosca-inclinada"] = new() { Name = "Rosca Inclinada", Series = 3, Reps = "10–12", Rest = "60s", Muscle = "Bíceps" },
            ["rosca-spider"] = new() { Name = "Rosca Spider", Series = 3, Reps = "10–12", Rest = "60s", Muscle = "Bíceps" },
            ["rosca-alternada"] = new() { Name = "Rosca Alternada", Series = 3, Reps = "10–12", Rest = "60s", Muscle = "Bíceps" },
            ["rosca-scott"] = new() { Name = "Rosca Scott", Series = 3, Reps = "8–12", Rest = "60s", Muscle = "Bíceps" },
            ["rosca-martelo"] = new() { Name = "Rosca Martelo", Series = 3, Reps = "8–12", Rest = "60s", Muscle = "Braquial & Braquiorradial" },
            ["abdominal"] = new() { Name = "Abdominal Reto", Series = 3, Reps = "15–20", Rest = "45s", Muscle = "Core" },
            ["prancha"] = new() { Name = "Prancha Isométrica", Series = 3, Reps = "45–60s", Rest = "45s", Muscle = "Core" },
            ["abdominal-supra"] = new() { Name = "Abdominal Supra", Series = 3, Reps = "15–20", Rest = "60s", Muscle = "Core" },
            ["abdominal-infra"] = new() { Name = "Abdominal Infra", Series = 3, Reps = "15–20", Rest = "60s", Muscle = "Core" },
            ["abdominal-bicicleta"] = new() { Name = "Abdominal Bicicleta", Series = 3, Reps = "15–20", Rest = "60s", Muscle = "Core" },
            ["abdominal-cruzada"] = new() { Name = "Abdominal Cruzada", Series = 3, Reps = "15–20", Rest = "45s", Muscle = "Core" },
            ["abdominal-alpinista"] = new() { Name = "Abdominal Alpinista", Series = 3, Reps = "15–20", Rest = "60s", Muscle = "Core" },
            ["prancha-rotacao"] = new() { Name = "Prancha com Rotação", Series = 3, Reps = "10 cada", Rest = "40s", Muscle = "Core" },
            ["stiff"] = new() { Name = "Stiff com Barra", Series = 3, Reps = "10–12", Rest = "90s", Muscle = "Posterior & Glúteo" },
            ["cadeira-flexora"] = new() { Name = "Cadeira Flexora", Series = 3, Reps = "12–15", Rest = "60s", Muscle = "Posterior & Isquiotibiais" },
            ["mesa-flexora"] = new() { Name = "Mesa Flexora", Series = 3, Reps = "10–12", Rest = "60s", Muscle = "Posterior & Isquiotibiais" },
            ["back-extension"] = new() { Name = "Extensão de Coluna", Series = 3, Reps = "8–10", Rest = "60s", Muscle = "Posterior & Lombar" },
            ["terra"] = new() { Name = "Levantamento Terra", Series = 4, Reps = "8–10", Rest = "120s", Muscle = "Posterior" },
            ["agachamento"] = new() { Name = "Agachamento Livre", Series = 4, Reps = "6–10", Rest = "120s", Muscle = "Quadríceps & Glúteo" },
            ["agachamento-sumo"] = new() { Name = "Agachamento Sumo", Series = 4, Reps = "6–10", Rest = "120s", Muscle = "Quadríceps & Glúteo" },
            ["agachamento-bulgaro"] = new() { Name = "Agachamento Bulgaro", Series = 4, Reps = "6–10", Rest = "120s", Muscle = "Quadríceps & Glúteo" },
            ["cadeira-extensora"] = new() { Name = "Cadeira Extensora", Series = 3, Reps = "12–15", Rest = "60s", Muscle = "Quadríceps" },
            ["leg-press"] = new() { Name = "Leg Press 45°", Series = 4, Reps = "8–12", Rest = "60s", Muscle = "Quadríceps" },
            ["agachamento-goblet"] = new() { Name = "Agachamento Goblet", Series = 3, Reps = "20–25", Rest = "45s", Muscle = "Quadríceps" },
            ["agachamento-hack"] = new() { Name = "Agachamento Hack", Series = 4, Reps = "8–10", Rest = "60s", Muscle = "Quadríceps" },
            ["agachamento-smith"] = new() { Name = "Agachamento Smith", Series = 4, Reps = "8–12", Rest = "90s", Muscle = "Quadríceps & Glúteo" },
            ["elevacao-pelvica"] = new() { Name = "Elevação Pélvica", Series = 4, Reps = "8–12", Rest = "60s", Muscle = "Glúteo" },
            ["extensao-quadril"] = new() { Name = "Extensão de Quadril na Polia", Series = 4, Reps = "10–15", Rest = "45s", Muscle = "Glúteo" },
            ["afundo"] = new() { Name = "Afundo Alternado", Series = 4, Reps = "12 cada", Rest = "45s", Muscle = "Quadríceps & Glúteo" },
            ["panturrilha"] = new() { Name = "Panturrilha em Pé", Series = 4, Reps = "15–20", Rest = "45s", Muscle = "Panturrilhas" },
            ["agachamento-salto"] = new() { Name = "Agachamento com Salto", Series = 4, Reps = "15–20", Rest = "40s", Muscle = "Full body" },
            ["burpee"] = new() { Name = "Burpee Modificado", Series = 3, Reps = "10–12", Rest = "45s", Muscle = "Full body" },
            ["mountain-climber"] = new() { Name = "Mountain Climber", Series = 3, Reps = "20–25", Rest = "40s", Muscle = "Core & Cardiovascular" },
            ["kettlebell-swing"] = new() { Name = "Kettlebell Swing", Series = 4, Reps = "20–25", Rest = "60s", Muscle = "Posterior chain" },
            ["jumping-jacks"] = new() { Name = "Jumping Jacks", Series = 3, Reps = "30s", Rest = "30s", Muscle = "Cardiovascular" },
            ["corrida-intervalada"] = new() { Name = "Corrida Intervalada", Series = 5, Reps = "1 min forte", Rest = "1 min leve", Muscle = "Cardiovascular" },
            ["pulo-corda"] = new() { Name = "Pulo de Corda", Series = 3, Reps = "2 min", Rest = "60s", Muscle = "Cardiovascular" },
        };
    }

    private static Dictionary<string, string[]> GetHardcodedFocusMapping()
    {
        return new()
        {
            ["Full Body"] = new[] { "agachamento", "supino-reto", "puxada-frontal", "desenvolvimento", "rosca-direta", "triceps-corda", "abdominal" },
            ["Superiores"] = new[] { "supino-inclinado", "puxada-frontal", "crucifixo-reto", "remada-curvada", "elevacao-lateral", "rosca-alternada", "triceps-frances", "crucifixo-inclinado", "voador-frontal", "crossover-polia-alta", "crossover-polia-baixa", "triceps-testa", "triceps-pulley", "puxada-alta", "remada-cavalinho", "pulldown", "crucifixo-inverso", "rosca-martelo", "rosca-scott", "rosca-direta-polia", "rosca-concentrada", "rosca-inclinada", "rosca-spider", "desenvolvimento", "elevacao-frontal", "arnold-press", "remada-alta", "face-pull" },
            ["Inferiores"] = new[] { "agachamento", "leg-press", "cadeira-extensora", "cadeira-flexora", "stiff", "elevacao-pelvica", "panturrilha", "abdominal", "agachamento-sumo", "agachamento-bulgaro", "agachamento-goblet", "agachamento-hack", "agachamento-smith", "mesa-flexora", "extensao-quadril", "afundo", "abdominal-supra", "abdominal-infra", "abdominal-bicicleta", "abdominal-cruzada", "prancha", "prancha-rotacao" },
            ["Ombros & Core"] = new[] { "desenvolvimento", "elevacao-lateral", "elevacao-frontal", "face-pull", "remada-alta", "prancha", "abdominal-bicicleta", "arnold-press", "abdominal", "abdominal-supra", "abdominal-infra", "abdominal-cruzada", "abdominal-alpinista", "prancha-rotacao" },
            ["Peito & Tríceps"] = new[] { "supino-inclinado", "supino-reto", "crucifixo-inclinado", "crucifixo-reto", "voador-frontal", "crossover-polia-alta", "crossover-polia-baixa", "triceps-testa", "triceps-frances", "triceps-pulley", "crossover-polia-media", "triceps-corda", "triceps-coice", "flexao-fechada", "mergulho", "supino-declinado" },
            ["Costas & Bíceps"] = new[] { "puxada-alta", "remada-curvada", "remada-cavalinho", "remada-unilateral", "puxada-frontal", "terra", "rosca-direta", "rosca-martelo", "rosca-scott", "pulldown", "crucifixo-inverso", "rosca-alternada", "rosca-concentrada", "rosca-inclinada", "rosca-spider", "rosca-direta-polia" },
            ["Pernas & Glúteos"] = new[] { "agachamento", "leg-press", "cadeira-extensora", "stiff", "afundo", "agachamento-goblet", "cadeira-flexora", "panturrilha", "elevacao-pelvica", "extensao-quadril", "agachamento-sumo", "agachamento-bulgaro", "agachamento-hack", "agachamento-smith", "mesa-flexora", "abdominal", "abdominal-supra", "abdominal-infra", "abdominal-bicicleta", "abdominal-cruzada", "prancha", "prancha-rotacao" },
            ["Pernas & Ombros"] = new[] { "agachamento", "leg-press", "cadeira-extensora", "cadeira-flexora", "elevacao-lateral", "desenvolvimento", "elevacao-frontal", "remada-alta", "stiff", "elevacao-pelvica", "panturrilha", "face-pull", "arnold-press", "afundo" },
            ["Full Body Metabolic"] = new[] { "agachamento-salto", "flexao", "burpee", "mountain-climber", "afundo", "remada-unilateral", "prancha-rotacao", "jumping-jacks" },
            ["HIIT + Resistência"] = new[] { "afundo", "remada-unilateral", "prancha-rotacao", "jumping-jacks", "burpee", "mountain-climber", "kettlebell-swing", "corrida-intervalada" },
            ["Agachamento + Empurrar"] = new[] { "agachamento", "supino-reto", "desenvolvimento", "supino-inclinado", "leg-press", "triceps-corda" },
            ["Terra + Puxar"] = new[] { "terra", "puxada-alta", "remada-cavalinho", "remada-curvada", "puxada-frontal", "rosca-direta", "rosca-martelo" },
            ["Resistência Muscular"] = new[] { "agachamento-goblet", "flexao", "remada-unilateral", "prancha", "agachamento-salto", "panturrilha", "abdominal" },
            ["Cardio + Força"] = new[] { "corrida-intervalada", "kettlebell-swing", "pulo-corda", "burpee", "jumping-jacks", "mountain-climber" },
        };
    }

    private static Dictionary<string, WorkoutPlan> GetHardcodedWorkoutTemplates()
    {
        return new()
        {
            ["HipertrofiaFullbody"] = new()
            {
                Nome = "Protocolo Hypertrophy Pro — Fullbody",
                Desc = "Treino completo para iniciantes em hipertrofia",
                Splits = new()
                {
                    new() { Label = "Treino Fullbody", Focus = "Full Body", Exercises = new() { new() { Name = "Agachamento Livre", Muscle = "Quadríceps & Glúteo", Series = 4, Reps = "8–12", Rest = "90s" }, new() { Name = "Supino Reto Máquina ou Barra", Muscle = "Peitoral maior", Series = 3, Reps = "8–12", Rest = "60s" }, new() { Name = "Puxada Frontal", Muscle = "Costas", Series = 3, Reps = "8–12", Rest = "60s" }, new() { Name = "Desenvolvimento Máquina ou Halter", Muscle = "Deltoides", Series = 3, Reps = "8–12", Rest = "60s" }, new() { Name = "Rosca Direta com Barra", Muscle = "Bíceps", Series = 3, Reps = "8–12", Rest = "60s" }, new() { Name = "Tríceps Corda na Polia", Muscle = "Tríceps", Series = 3, Reps = "8–12", Rest = "60s" }, new() { Name = "Abdominal Reto", Muscle = "Core", Series = 3, Reps = "15–20", Rest = "45s" } } }
                }
            },
            ["HipertrofiaAB"] = new()
            {
                Nome = "Protocolo Hypertrophy Pro — Upper/Lower",
                Desc = "Divisão AB para hipertrofia (superiores/inferiores)",
                Splits = new()
                {
                    new() { Label = "Treino A", Focus = "Superiores", Exercises = new() { new() { Name = "Supino Inclinado Máquina ou Barra", Muscle = "Peitoral superior", Series = 3, Reps = "8–12", Rest = "60s" }, new() { Name = "Puxada Frontal", Muscle = "Costas", Series = 4, Reps = "8–12", Rest = "60s" }, new() { Name = "Crucifixo Reto", Muscle = "Peitoral maior", Series = 3, Reps = "8–12", Rest = "60s" }, new() { Name = "Remada Curvada com Barra", Muscle = "Costas & Lombar", Series = 3, Reps = "8–12", Rest = "75s" }, new() { Name = "Elevação Lateral Polia ou Halter", Muscle = "Deltoides medial e posterior", Series = 3, Reps = "8–12", Rest = "60s" }, new() { Name = "Rosca Alternada", Muscle = "Bíceps", Series = 3, Reps = "10–12", Rest = "60s" }, new() { Name = "Tríceps Francês Corda ou Halter", Muscle = "Tríceps", Series = 3, Reps = "10–12", Rest = "60s" } } },
                    new() { Label = "Treino B", Focus = "Inferiores", Exercises = new() { new() { Name = "Agachamento Livre", Muscle = "Quadríceps & Glúteo", Series = 4, Reps = "6–10", Rest = "120s" }, new() { Name = "Leg Press 45°", Muscle = "Quadríceps", Series = 3, Reps = "8–12", Rest = "60s" }, new() { Name = "Cadeira Extensora", Muscle = "Quadríceps", Series = 3, Reps = "12–15", Rest = "60s" }, new() { Name = "Cadeira Flexora", Muscle = "Posterior & Isquiotibiais", Series = 3, Reps = "12–15", Rest = "60s" }, new() { Name = "Stiff com Barra", Muscle = "Posterior & Glúteo", Series = 3, Reps = "10–12", Rest = "90s" }, new() { Name = "Elevação Pélvica", Muscle = "Glúteo", Series = 4, Reps = "8–12", Rest = "60s" }, new() { Name = "Panturrilha em Pé", Muscle = "Panturrilhas", Series = 4, Reps = "15–20", Rest = "45s" }, new() { Name = "Abdominal Reto", Muscle = "Core", Series = 3, Reps = "15–20", Rest = "45s" } } }
                }
            },
            ["HipertrofiaABC"] = new()
            {
                Nome = "Protocolo Hypertrophy Pro — Push/Pull/Legs",
                Desc = "Divisão ABC para hipertrofia (empurrar/puxar/pernas)",
                Splits = new()
                {
                    new() { Label = "Treino A", Focus = "Peito & Tríceps", Exercises = new() { new() { Name = "Supino Inclinado Máquina ou Barra", Muscle = "Peitoral superior", Series = 3, Reps = "8-12", Rest = "60s" }, new() { Name = "Crucifixo Reto", Muscle = "Peitoral maior", Series = 3, Reps = "8–10", Rest = "60s" }, new() { Name = "Voador Frontal", Muscle = "Peitoral maior", Series = 3, Reps = "8-10", Rest = "60s" }, new() { Name = "Cross Over Polia Alta", Muscle = "Peitoral superior", Series = 3, Reps = "8-10", Rest = "60s" }, new() { Name = "Cross Over Polia Baixa", Muscle = "Peitoral inferior", Series = 3, Reps = "8-10", Rest = "60s" }, new() { Name = "Tríceps Polia Alta", Muscle = "Tríceps", Series = 4, Reps = "8-10", Rest = "60s" }, new() { Name = "Tríceps Francês", Muscle = "Tríceps", Series = 4, Reps = "10-12", Rest = "60s" } } },
                    new() { Label = "Treino B", Focus = "Costas & Bíceps", Exercises = new() { new() { Name = "Puxada Frontal", Muscle = "Costas", Series = 4, Reps = "8–12", Rest = "90s" }, new() { Name = "Remada Curvada com Barra", Muscle = "Costas & Lombar", Series = 3, Reps = "8–12", Rest = "75s" }, new() { Name = "Crucifixo Inverso", Muscle = "Costas & Ombros", Series = 3, Reps = "8–10", Rest = "60s" }, new() { Name = "Pulldown", Muscle = "Costas", Series = 3, Reps = "8-10", Rest = "60s" }, new() { Name = "Rosca Martelo", Muscle = "Braquial & Braquiorradial", Series = 3, Reps = "8-12", Rest = "60s" }, new() { Name = "Rosca Alternada", Muscle = "Bíceps", Series = 3, Reps = "10-12", Rest = "60s" } } },
                    new() { Label = "Treino C", Focus = "Pernas & Ombros", Exercises = new() { new() { Name = "Agachamento Livre", Muscle = "Quadríceps & Glúteo", Series = 4, Reps = "6–10", Rest = "120s" }, new() { Name = "Leg Press 45°", Muscle = "Quadríceps", Series = 3, Reps = "8-12", Rest = "90s" }, new() { Name = "Cadeira Extensora", Muscle = "Quadríceps", Series = 3, Reps = "12–15", Rest = "60s" }, new() { Name = "Cadeira Flexora", Muscle = "Posterior & Isquiotibiais", Series = 3, Reps = "8-12", Rest = "60s" }, new() { Name = "Elevação Lateral Polia ou Halter", Muscle = "Deltoides medial e posterior", Series = 3, Reps = "8-12", Rest = "60s" }, new() { Name = "Desenvolvimento Máquina ou Halter", Muscle = "Deltoides", Series = 3, Reps = "8-12", Rest = "60s" }, new() { Name = "Elevação Frontal", Muscle = "Deltoides frontal", Series = 3, Reps = "8-12", Rest = "60s" }, new() { Name = "Remada Alta", Muscle = "Deltoides", Series = 3, Reps = "8-12", Rest = "60s" } } }
                }
            },
            ["HipertrofiaABCD"] = new()
            {
                Nome = "Protocolo Hypertrophy Pro — ABCD",
                Desc = "Divisão de 4 dias para hipertrofia avançada",
                Splits = new()
                {
                    new() { Label = "Treino A", Focus = "Peito & Tríceps", Exercises = new() { new() { Name = "Supino Reto Máquina ou Barra", Muscle = "Peitoral maior", Series = 4, Reps = "6–10", Rest = "90s" }, new() { Name = "Supino Inclinado Máquina ou Barra", Muscle = "Peitoral superior", Series = 4, Reps = "8–12", Rest = "60s" }, new() { Name = "Crucifixo Inclinado Máquina ou Halter", Muscle = "Peitoral superior", Series = 3, Reps = "8–12", Rest = "60s" }, new() { Name = "Cross Over Polia Média", Muscle = "Peitoral maior", Series = 3, Reps = "8–12", Rest = "60s" }, new() { Name = "Tríceps Testa Corda ou Barra W", Muscle = "Tríceps", Series = 3, Reps = "10–12", Rest = "60s" }, new() { Name = "Tríceps Corda na Polia", Muscle = "Tríceps", Series = 3, Reps = "8-12", Rest = "60s" } } },
                    new() { Label = "Treino B", Focus = "Costas & Bíceps", Exercises = new() { new() { Name = "Barra Fixa Pegada Larga", Muscle = "Latíssimo do dorso", Series = 4, Reps = "6–10", Rest = "90s" }, new() { Name = "Remada Curvada com Barra", Muscle = "Costas & Lombar", Series = 4, Reps = "8–12", Rest = "75s" }, new() { Name = "Puxada Frontal", Muscle = "Costas", Series = 3, Reps = "8–12", Rest = "60s" }, new() { Name = "Crucifixo Inverso", Muscle = "Costas & Ombros", Series = 3, Reps = "8–10", Rest = "60s" }, new() { Name = "Rosca Direta com Barra", Muscle = "Bíceps", Series = 3, Reps = "6–8", Rest = "60s" }, new() { Name = "Rosca Martelo", Muscle = "Braquial & Braquiorradial", Series = 3, Reps = "8-12", Rest = "60s" } } },
                    new() { Label = "Treino C", Focus = "Pernas & Glúteos", Exercises = new() { new() { Name = "Agachamento Livre", Muscle = "Quadríceps & Glúteo", Series = 4, Reps = "6–10", Rest = "120s" }, new() { Name = "Leg Press 45°", Muscle = "Quadríceps", Series = 4, Reps = "8–12", Rest = "60s" }, new() { Name = "Cadeira Extensora", Muscle = "Quadríceps", Series = 3, Reps = "12–15", Rest = "60s" }, new() { Name = "Cadeira Flexora", Muscle = "Posterior & Isquiotibiais", Series = 3, Reps = "12–15", Rest = "60s" }, new() { Name = "Elevação Pélvica", Muscle = "Glúteo", Series = 4, Reps = "8–12", Rest = "60s" }, new() { Name = "Panturrilha em Pé", Muscle = "Panturrilhas", Series = 4, Reps = "15–20", Rest = "45s" }, new() { Name = "Abdominal Reto", Muscle = "Core", Series = 3, Reps = "15–20", Rest = "45s" } } },
                    new() { Label = "Treino D", Focus = "Ombros & Core", Exercises = new() { new() { Name = "Desenvolvimento Máquina ou Halter", Muscle = "Deltoides", Series = 4, Reps = "8–12", Rest = "60s" }, new() { Name = "Elevação Lateral Polia ou Halter", Muscle = "Deltoides medial e posterior", Series = 4, Reps = "8–12", Rest = "60s" }, new() { Name = "Elevação Frontal", Muscle = "Deltoides frontal", Series = 3, Reps = "8–12", Rest = "60s" }, new() { Name = "Face Pull", Muscle = "Deltoides", Series = 4, Reps = "8–10", Rest = "90s" }, new() { Name = "Remada Alta", Muscle = "Deltoides", Series = 3, Reps = "8–10", Rest = "60s" }, new() { Name = "Prancha Isométrica", Muscle = "Core", Series = 3, Reps = "45–60s", Rest = "45s" }, new() { Name = "Abdominal Bicicleta", Muscle = "Core", Series = 3, Reps = "15–20", Rest = "60s" } } }
                }
            },
            ["Emagrecimento"] = new()
            {
                Nome = "Protocolo Fat Burn Circuit",
                Desc = "Circuito metabólico de alta densidade para queima de gordura",
                Splits = new()
                {
                    new() { Label = "Circuito A", Focus = "Full Body Metabolic", Exercises = new() { new() { Name = "Agachamento com Salto", Muscle = "Full body", Series = 4, Reps = "15–20", Rest = "40s" }, new() { Name = "Flexão de Braços", Muscle = "Peito & Ombros", Series = 3, Reps = "12–15", Rest = "40s" }, new() { Name = "Burpee Modificado", Muscle = "Full body", Series = 3, Reps = "10–12", Rest = "45s" }, new() { Name = "Mountain Climber", Muscle = "Core & Cardiovascular", Series = 3, Reps = "20–25", Rest = "40s" } } },
                    new() { Label = "Circuito B", Focus = "HIIT + Resistência", Exercises = new() { new() { Name = "Afundo Alternado", Muscle = "Quadríceps & Glúteo", Series = 4, Reps = "12 cada", Rest = "45s" }, new() { Name = "Remada com Halteres", Muscle = "Costas & Bíceps", Series = 3, Reps = "15–18", Rest = "40s" }, new() { Name = "Prancha com Rotação", Muscle = "Core", Series = 3, Reps = "10 cada", Rest = "40s" }, new() { Name = "Jumping Jacks", Muscle = "Cardiovascular", Series = 3, Reps = "30s", Rest = "30s" } } }
                }
            },
            ["Forca"] = new()
            {
                Nome = "Protocolo Strength Builder",
                Desc = "Periodização linear para ganho de força máxima",
                Splits = new()
                {
                    new() { Label = "Treino A", Focus = "Agachamento + Empurrar", Exercises = new() { new() { Name = "Agachamento Livre", Muscle = "Quadríceps & Glúteo", Series = 5, Reps = "3–5", Rest = "180s" }, new() { Name = "Supino Reto com Barra", Muscle = "Peitoral", Series = 5, Reps = "3–5", Rest = "180s" }, new() { Name = "Desenvolvimento Militar", Muscle = "Deltoides", Series = 3, Reps = "5–6", Rest = "120s" } } },
                    new() { Label = "Treino B", Focus = "Terra + Puxar", Exercises = new() { new() { Name = "Levantamento Terra", Muscle = "Posterior chain", Series = 4, Reps = "3–4", Rest = "240s" }, new() { Name = "Barra Fixa Lastrada", Muscle = "Latíssimo", Series = 4, Reps = "4–6", Rest = "180s" }, new() { Name = "Remada Cavalinho", Muscle = "Costas", Series = 3, Reps = "5–6", Rest = "120s" } } }
                }
            },
            ["Resistencia"] = new()
            {
                Nome = "Protocolo Endurance Plus",
                Desc = "Volume progressivo para resistência muscular e aeróbica",
                Splits = new()
                {
                    new() { Label = "Sessão A", Focus = "Resistência Muscular", Exercises = new() { new() { Name = "Agachamento Goblet", Muscle = "Quadríceps", Series = 3, Reps = "20–25", Rest = "45s" }, new() { Name = "Flexão de Braços", Muscle = "Peito & Ombros", Series = 3, Reps = "15–20", Rest = "45s" }, new() { Name = "Remada com Halteres", Muscle = "Costas", Series = 3, Reps = "15–20", Rest = "45s" }, new() { Name = "Prancha Isométrica", Muscle = "Core", Series = 3, Reps = "45–60s", Rest = "45s" } } },
                    new() { Label = "Sessão B", Focus = "Cardio + Força", Exercises = new() { new() { Name = "Corrida Intervalada", Muscle = "Cardiovascular", Series = 5, Reps = "1 min forte", Rest = "1 min leve" }, new() { Name = "Kettlebell Swing", Muscle = "Posterior chain", Series = 4, Reps = "20–25", Rest = "60s" }, new() { Name = "Pulo de Corda", Muscle = "Cardiovascular", Series = 3, Reps = "2 min", Rest = "60s" } } }
                }
            },
        };
    }

    private class JsonExerciseItem
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Series { get; set; }
        public string Reps { get; set; } = string.Empty;
        public string Rest { get; set; } = string.Empty;
        public string Muscle { get; set; } = string.Empty;
    }
}