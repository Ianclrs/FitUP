// FitUP - Carregador dinâmico do jsPDF (lazy load)
// Evita baixar ~300 KB em páginas que não usam exportação PDF
window.fitUpPdfLoader = (function () {
    let loading = null;
    let loaded = false;

    async function ensureJsPdf() {
        if (loaded) return true;
        if (loading) return loading;
        loading = new Promise((resolve, reject) => {
            // Se já foi carregado de alguma forma, retorna imediatamente
            if (window.jspdf && window.jspdf.jsPDF) {
                loaded = true;
                resolve(true);
                return;
            }
            const script = document.createElement('script');
            script.src = '/js/jspdf.umd.min.js';
            script.onload = () => {
                loaded = true;
                resolve(true);
            };
            script.onerror = () => {
                loading = null;
                reject(new Error('Falha ao carregar jsPDF'));
            };
            document.head.appendChild(script);
        });
        return loading;
    }

    return { ensureJsPdf };
})();