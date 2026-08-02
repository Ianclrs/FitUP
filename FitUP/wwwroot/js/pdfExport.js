// FitUP - Exportação de Treino para PDF
// Depende de: jsPDF (window.jspdf) — carregado dinamicamente via pdfLoader.js

async function exportFitUpPdf(workoutJson) {
    try {
        // Garante que o jsPDF está carregado (lazy load)
        if (window.fitUpPdfLoader) {
            await window.fitUpPdfLoader.ensureJsPdf();
        }
        const data = JSON.parse(workoutJson);
        const { jsPDF } = window.jspdf;
        const doc = new jsPDF({ orientation: 'portrait', unit: 'mm', format: 'a4' });

        const pageW = doc.internal.pageSize.getWidth();
        const margin = 16;
        const contentW = pageW - margin * 2;
        let y = 20;

        // ── Cores ──
        const orange = [255, 122, 47];
        const dark = [33, 33, 33];
        const gray = [130, 130, 130];
        const lightGray = [235, 235, 235];

        // ── Cabeçalho ──
        doc.setFont('helvetica', 'bold');
        doc.setFontSize(22);
        doc.setTextColor(orange[0], orange[1], orange[2]);
        doc.text('FitUP', margin, y);
        doc.setFontSize(10);
        doc.setTextColor(gray[0], gray[1], gray[2]);
        doc.text('Seu treino personalizado', margin, y + 5);
        doc.setDrawColor(orange[0], orange[1], orange[2]);
        doc.setLineWidth(0.6);
        doc.line(margin, y + 8, pageW - margin, y + 8);

        y += 16;

        // ── Nome do treino ──
        doc.setFontSize(16);
        doc.setFont('helvetica', 'bold');
        doc.setTextColor(dark[0], dark[1], dark[2]);
        doc.text(data.nome, margin, y);
        y += 7;

        // ── Descrição ──
        doc.setFontSize(11);
        doc.setFont('helvetica', 'normal');
        doc.setTextColor(gray[0], gray[1], gray[2]);
        doc.text(data.desc, margin, y);
        y += 5;

        // ── Metadados ──
        doc.setFontSize(10);
        const metaParts = [];
        if (data.nivel) metaParts.push('Nível: ' + data.nivel);
        if (data.local) metaParts.push('Local: ' + data.local);
        if (data.duracao) metaParts.push('Duração: ' + data.duracao);
        if (data.diasSemana) metaParts.push('Frequência: ' + data.diasSemana + 'x/semana');
        doc.text(metaParts.join('  |  '), margin, y);
        y += 8;

        // ── Linha separadora ──
        doc.setDrawColor(lightGray[0], lightGray[1], lightGray[2]);
        doc.setLineWidth(0.3);
        doc.line(margin, y, pageW - margin, y);
        y += 8;

        // ── Splits de treino ──
        if (!data.splits || data.splits.length === 0) {
            doc.setFontSize(12);
            doc.setTextColor(gray[0], gray[1], gray[2]);
            doc.text('Nenhum treino encontrado.', margin, y);
        } else {
            for (let s = 0; s < data.splits.length; s++) {
                const split = data.splits[s];

                // Verifica se precisa de nova página
                if (y > 240) {
                    doc.addPage();
                    y = 20;
                }

                // ── Título do split ──
                doc.setFillColor(orange[0], orange[1], orange[2]);
                doc.roundedRect(margin, y, contentW, 10, 2, 2, 'F');
                doc.setFont('helvetica', 'bold');
                doc.setFontSize(11);
                doc.setTextColor(255, 255, 255);
                doc.text(split.label, margin + 4, y + 7);

                // Foco
                doc.setFontSize(8);
                doc.setFont('helvetica', 'normal');
                doc.text('[Foco: ' + split.focus + ']', margin + contentW - 4, y + 7, { align: 'right' });

                y += 14;

                // ── Cabeçalho da tabela ──
                const colX = [margin, margin + 6, margin + 66, margin + 104, margin + 126, margin + 148];
                const colW = [6, 60, 38, 22, 22, 26];

                doc.setFillColor(lightGray[0], lightGray[1], lightGray[2]);
                doc.rect(margin, y, contentW, 7, 'F');
                doc.setFontSize(8);
                doc.setFont('helvetica', 'bold');
                doc.setTextColor(dark[0], dark[1], dark[2]);

                const headers = ['#', 'Exercício', 'Músculo', 'Séries', 'Reps', 'Descanso'];
                for (let h = 0; h < headers.length; h++) {
                    doc.text(headers[h], colX[h], y + 5);
                }
                y += 9;

                // ── Exercícios ──
                if (!split.exercicios || split.exercicios.length === 0) {
                    doc.setFontSize(9);
                    doc.setFont('helvetica', 'normal');
                    doc.setTextColor(gray[0], gray[1], gray[2]);
                    doc.text('Nenhum exercício cadastrado.', margin + 4, y);
                    y += 5;
                } else {
                    for (let e = 0; e < split.exercicios.length; e++) {
                        const ex = split.exercicios[e];

                        if (y > 270) {
                            doc.addPage();
                            y = 20;
                            // Reimprime cabeçalho da tabela na nova página
                            doc.setFillColor(lightGray[0], lightGray[1], lightGray[2]);
                            doc.rect(margin, y, contentW, 7, 'F');
                            doc.setFontSize(8);
                            doc.setFont('helvetica', 'bold');
                            doc.setTextColor(dark[0], dark[1], dark[2]);
                            for (let h = 0; h < headers.length; h++) {
                                doc.text(headers[h], colX[h], y + 5);
                            }
                            y += 9;
                        }

                        // Linha zebrada
                        if (e % 2 === 0) {
                            doc.setFillColor(250, 250, 250);
                            doc.rect(margin, y - 1, contentW, 7, 'F');
                        }

                        doc.setFontSize(8);
                        doc.setFont('helvetica', 'normal');
                        doc.setTextColor(dark[0], dark[1], dark[2]);

                        doc.text(String(e + 1), colX[0], y + 4);
                        doc.text(ex.name || '', colX[1], y + 4);
                        doc.text(ex.muscle || '', colX[2], y + 4);
                        doc.text(String(ex.series || '-'), colX[3], y + 4);
                        doc.text(ex.reps || '-', colX[4], y + 4);
                        doc.text(ex.rest || '-', colX[5], y + 4);

                        y += 7;

                        // Linha separadora fina entre exercícios
                        doc.setDrawColor(230, 230, 230);
                        doc.setLineWidth(0.1);
                        doc.line(margin, y, pageW - margin, y);
                        y += 1;
                    }
                }

                y += 6;
            }
        }

        // ── Rodapé ──
        const totalPages = doc.getNumberOfPages();
        for (let p = 1; p <= totalPages; p++) {
            doc.setPage(p);
            doc.setFontSize(8);
            doc.setFont('helvetica', 'normal');
            doc.setTextColor(gray[0], gray[1], gray[2]);
            doc.text('FitUP © ' + new Date().getFullYear() + '  —  Página ' + p + ' de ' + totalPages, pageW / 2, doc.internal.pageSize.getHeight() - 10, { align: 'center' });
        }

        // ── Salvar ──
        const fileName = (data.nome || 'Treino_FitUP')
            .replace(/[^a-zA-Z0-9À-ÿ\s-]/g, '')
            .replace(/\s+/g, '_')
            .slice(0, 50) + '.pdf';

        doc.save(fileName);
        return true;
    } catch (err) {
        console.error('FitUP PDF export error:', err);
        return false;
    }
}

// ──────────────────────────────────────────────────────────
// FitUP - Exportação de Dieta para PDF
// ──────────────────────────────────────────────────────────
async function exportDietaPdf(dietaJson) {
    try {
        // Garante que o jsPDF está carregado (lazy load)
        if (window.fitUpPdfLoader) {
            await window.fitUpPdfLoader.ensureJsPdf();
        }
        const data = JSON.parse(dietaJson);
        const { jsPDF } = window.jspdf;
        const doc = new jsPDF({ orientation: 'portrait', unit: 'mm', format: 'a4' });

        const pageW = doc.internal.pageSize.getWidth();
        const margin = 16;
        const contentW = pageW - margin * 2;
        let y = 20;

        // ── Cores ──
        const orange = [255, 122, 47];
        const dark = [33, 33, 33];
        const gray = [130, 130, 130];
        const lightGray = [235, 235, 235];

        // ── Cabeçalho ──
        doc.setFont('helvetica', 'bold');
        doc.setFontSize(22);
        doc.setTextColor(orange[0], orange[1], orange[2]);
        doc.text('FitUP', margin, y);
        doc.setFontSize(10);
        doc.setTextColor(gray[0], gray[1], gray[2]);
        doc.text('Seu plano alimentar personalizado', margin, y + 5);
        doc.setDrawColor(orange[0], orange[1], orange[2]);
        doc.setLineWidth(0.6);
        doc.line(margin, y + 8, pageW - margin, y + 8);

        y += 16;

        // ── Nome da dieta ──
        doc.setFontSize(16);
        doc.setFont('helvetica', 'bold');
        doc.setTextColor(dark[0], dark[1], dark[2]);
        doc.text(data.nome, margin, y);
        y += 7;

        // ── Descrição ──
        doc.setFontSize(10);
        doc.setFont('helvetica', 'normal');
        doc.setTextColor(gray[0], gray[1], gray[2]);

        // Quebra de linha automática para descrição
        const descLines = doc.splitTextToSize(data.descricao, contentW);
        for (let dl = 0; dl < descLines.length; dl++) {
            doc.text(descLines[dl], margin, y);
            y += 5;
        }
        y += 4;

        // ── Linha separadora ──
        doc.setDrawColor(lightGray[0], lightGray[1], lightGray[2]);
        doc.setLineWidth(0.3);
        doc.line(margin, y, pageW - margin, y);
        y += 10;

        // ── Refeições ──
        if (!data.refeicoes || data.refeicoes.length === 0) {
            doc.setFontSize(12);
            doc.setTextColor(gray[0], gray[1], gray[2]);
            doc.text('Nenhuma refeição encontrada.', margin, y);
        } else {
            for (let r = 0; r < data.refeicoes.length; r++) {
                const refeicao = data.refeicoes[r];

                // Verifica se precisa de nova página (precisa de pelo menos 30mm para o título + alguns itens)
                if (y > 255) {
                    doc.addPage();
                    y = 20;
                }

                // ── Título da refeição ──
                doc.setFillColor(orange[0], orange[1], orange[2]);
                doc.roundedRect(margin, y, contentW, 9, 2, 2, 'F');
                doc.setFont('helvetica', 'bold');
                doc.setFontSize(11);
                doc.setTextColor(255, 255, 255);
                doc.text(refeicao.titulo, margin + 4, y + 6.5);

                y += 13;

                // ── Alimentos ──
                if (!refeicao.alimentos || refeicao.alimentos.length === 0) {
                    doc.setFontSize(9);
                    doc.setFont('helvetica', 'normal');
                    doc.setTextColor(gray[0], gray[1], gray[2]);
                    doc.text('Nenhum alimento listado.', margin + 4, y);
                    y += 5;
                } else {
                    for (let a = 0; a < refeicao.alimentos.length; a++) {
                        const alimento = refeicao.alimentos[a];

                        if (y > 275) {
                            doc.addPage();
                            y = 20;
                        }

                        // Linha zebrada
                        if (a % 2 === 0) {
                            doc.setFillColor(250, 250, 250);
                            doc.rect(margin + 2, y - 1, contentW - 4, 7, 'F');
                        }

                        doc.setFontSize(9);
                        doc.setFont('helvetica', 'normal');

                        // Verifica se o alimento tem um label (ex: "Proteína:") em negrito
                        if (alimento.label) {
                            doc.setFont('helvetica', 'bold');
                            doc.setTextColor(dark[0], dark[1], dark[2]);
                            doc.text(alimento.label, margin + 6, y + 4);
                            const labelW = doc.getTextWidth(alimento.label);
                            doc.setFont('helvetica', 'normal');
                            doc.setTextColor(gray[0], gray[1], gray[2]);
                            doc.text(alimento.valor, margin + 6 + labelW, y + 4);
                        } else {
                            doc.setTextColor(dark[0], dark[1], dark[2]);
                            doc.text('• ' + (alimento.valor || alimento), margin + 6, y + 4);
                        }

                        y += 7;

                        // Linha separadora fina
                        doc.setDrawColor(230, 230, 230);
                        doc.setLineWidth(0.1);
                        doc.line(margin + 2, y, pageW - margin - 2, y);
                        y += 1;
                    }
                }

                y += 6;
            }
        }

        // ── Rodapé ──
        const totalPages = doc.getNumberOfPages();
        for (let p = 1; p <= totalPages; p++) {
            doc.setPage(p);
            doc.setFontSize(8);
            doc.setFont('helvetica', 'normal');
            doc.setTextColor(gray[0], gray[1], gray[2]);
            doc.text('FitUP © ' + new Date().getFullYear() + '  —  Página ' + p + ' de ' + totalPages, pageW / 2, doc.internal.pageSize.getHeight() - 10, { align: 'center' });
        }

        // ── Salvar ──
        const fileName = (data.nome || 'Dieta_FitUP')
            .replace(/[^a-zA-Z0-9À-ÿ\s-]/g, '')
            .replace(/\s+/g, '_')
            .slice(0, 50) + '.pdf';

        doc.save(fileName);
        return true;
    } catch (err) {
        console.error('FitUP Dieta PDF export error:', err);
        return false;
    }
}