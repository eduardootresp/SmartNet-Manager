using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SmartNet_Manager
{
    internal class Filas
    {
        private Queue<Dispositivo> _filaDeAutenticacaoDeDispositivos = new Queue<Dispositivo>();
        private List<Dispositivo> _listaDeDispositivosAtivos = new List<Dispositivo>();
        private Stack<string> _rollback = new Stack<string>();
        public void AdicionarDispositivoAFila(Dispositivo dispositivo)
        {
            if (dispositivo == null)
                throw new ArgumentNullException(nameof(dispositivo));

            _filaDeAutenticacaoDeDispositivos.Enqueue(dispositivo);
            Console.WriteLine($"Dispositivo '{dispositivo.Nome}' entrou na fila de Autenticação.");
        }

        public bool ProcessarFila()
        {
            if (_filaDeAutenticacaoDeDispositivos.Count == 0)
            {
                Console.WriteLine("A fila está vazia. Nenhum dispositivo para autenticaçâo");
                return false;
            }

            Dispositivo dispositivo = _filaDeAutenticacaoDeDispositivos.Dequeue();

            bool autenticado = VerificarSeguranca(dispositivo);

            if (autenticado)
            {
                _rollback.Push($"Dispositivo '{dispositivo.Nome}' autenticado e movido para ativos");
                Console.WriteLine($"Estado da stack: {string.Join(", ", _rollback.ToArray())}");

                Console.Write("Deseja manter a alteração? (S/N)");
                while (true)
                {
                    var resposta = Console.ReadLine()?.ToUpper();
                    if (resposta != "S" && resposta != "N")
                    {
                        Console.WriteLine("responda com 'S' ou 'N'");
                        Console.Write("Deseja manter a alteração? (S/N)");
                        continue;
                    }
                    if (resposta == "N")
                    {
                        DesfazerUltimaAcao(_rollback);
                        Console.WriteLine("Desfazendo a última ação");
                    }
                    else if (resposta == "S")
                    {
                        _listaDeDispositivosAtivos.Add(dispositivo); 
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Dispositivo '{dispositivo.Nome}' autenticado e movido para ativos.");
                        Console.ResetColor();
                        Console.WriteLine($"Estado da stack: {string.Join(", ", _rollback.ToArray())}");
                    }
                    break;
                }

            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Dispositivo '{dispositivo.Nome}' BLOQUEADO pela segurança.\"");
                Console.ResetColor();
            }
            return autenticado;
        }

        public void DesfazerUltimaAcao(Stack<string> acao)
        {
            var ultimaAcao = acao.Pop();
            Console.WriteLine($"Foi revertido a última ação: '{ultimaAcao.ToString()}'" );
        }

        private bool VerificarSeguranca(Dispositivo dispositivo)
        {
            if (dispositivo.Fabricante == "Apple" | dispositivo.Fabricante == "Samsung" | dispositivo.Fabricante == "Dell")
            {
                return true;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"O dispositivo({dispositivo.Nome} do(a) {dispositivo.Fabricante}) deve ser apenas de Fabricantes autorizados pela empresa.");
                Console.ResetColor();
                return false;
            }
        }

        public void MostrarFila()
        {
            Console.WriteLine($"\nFila de Autenticação ({_filaDeAutenticacaoDeDispositivos.Count} dispositivo(s)):");

            if (_filaDeAutenticacaoDeDispositivos.Count == 0)
            {
                Console.WriteLine("(vazia)");
                return;
            }

            int posicao = 1;
            foreach (var disp in _filaDeAutenticacaoDeDispositivos)
            {
                Console.WriteLine($" {posicao}- {disp.Nome}");
                posicao++;
            }
        }

        public void MostrarAtivos()
        {
            Console.WriteLine($"\nDispositivos Ativos ({_listaDeDispositivosAtivos.Count}):");
            if (_listaDeDispositivosAtivos.Count == 0)
            {
                Console.WriteLine("    (nenhum dispositivo ativo)");
                return;
            }

            foreach (var disp in _listaDeDispositivosAtivos)
            {
                Console.WriteLine($"- {disp.Nome}");
            }
        }

        public void ExibirDashBoard()
        {
            var quantidadeAtivos = _listaDeDispositivosAtivos.Count();
            var rankingConsumo = _listaDeDispositivosAtivos
                .OrderByDescending(x => x.ConsumoBanda)
                .ToList();

            Console.WriteLine("\n========= DASHBOARD DE REDE =========");

            // dispositivos ativos, 
            double consumoTotal = _listaDeDispositivosAtivos.Sum(d => d.ConsumoBanda);
            Console.WriteLine($"Consumo Total de Banda: {consumoTotal:F2} Mbps");
            Console.WriteLine("----------------------------------------");


            // Top 3 consumidores
            var top3Consumidores = _listaDeDispositivosAtivos
                .OrderByDescending(d =>  d.ConsumoBanda)
                .Take(3)
                .ToList();

            Console.WriteLine("Top 3 Maior Consumo: ");
            if (top3Consumidores.Count == 0)
            {
                Console.WriteLine("(Nenhum dispositivo ativo)");
            }
            else
            {
                int posicao = 1;
                foreach (var disp in top3Consumidores)
                {
                    Console.WriteLine($"{posicao}- {disp.Nome} ({disp.ConsumoBanda} Mbps)");
                    posicao++;
                }
            }
            Console.WriteLine("----------------------------------------");


            // Lista de dispositivos por fabricante
            Console.WriteLine($"Quantidade de dispositivos Ativos no total - {quantidadeAtivos}");
            Console.WriteLine("----------------------------------------");
            var porFabricante = _listaDeDispositivosAtivos
                .GroupBy(d => d.Fabricante)
                .OrderByDescending(x => x.Count());

            Console.WriteLine("Dispositivos por Fabricante:");
            foreach (var grupo in porFabricante)
            {
                Console.WriteLine($"  - {grupo.Key}: {grupo.Count()} unidade(s)");
            }
            Console.WriteLine("----------------------------------------");


            // Anomalias
            var anomalias = _listaDeDispositivosAtivos.Where(d => d.ConsumoBanda > 500).ToList();
            if (anomalias.Any())
            {
                foreach (var disp in anomalias)
                {
                    Console.WriteLine($"Alerta de alto consumo!!!\n-{disp.Nome}: {disp.ConsumoBanda}");
                }
            }
            else
            {
                Console.WriteLine($"Nenhum alerta de alto consumo!!!");
            }
            Console.WriteLine("----------------------------------------");
        }

        public void ImprimirRelatorio(IEnumerable<Dispositivo> colecao)
        {
            if (colecao == null)
                throw new ArgumentException(nameof(colecao));

            Console.WriteLine("\n╔══════════════════════════════════════╗");
            Console.WriteLine("║        RELATÓRIO DE DISPOSITIVOS     ║");
            Console.WriteLine("╚══════════════════════════════════════╝");

            int contador = 1;

            foreach (var item in colecao)
            {
                Console.WriteLine($" {contador}. {item.Nome}");
                contador++;
            }
            if (colecao.Count() == 0)
            {
                Console.WriteLine("(Relatório Vazio)");
            }
        }

        public void AplicarFirewall(Func<Dispositivo, bool> regraBloqueio)
        {
            if (regraBloqueio == null)
                throw new ArgumentNullException(nameof(regraBloqueio));

            // cópia da lista pra evitar b.o.
            var dispositivosParaAnalisar = _listaDeDispositivosAtivos.ToList();
            var bloqueados = new List<Dispositivo>();

            Console.WriteLine("===========Executando Firewall===========");

            foreach (var dispositivo in dispositivosParaAnalisar)
            {
                bool deveBloquear = regraBloqueio(dispositivo);

                if (deveBloquear)
                {
                    _rollback.Push($"Dispositivo '{dispositivo.Nome}' bloqueado");
                    Console.WriteLine($"Estado da stack: {string.Join(", ", _rollback.ToArray())}");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Dispositivo {dispositivo.Nome} Bloqueado.");
                    Console.ResetColor();
                    Console.Write("Deseja manter a alteração? (S/N)");
                    while (true)
                    {
                        var resposta = Console.ReadLine()?.ToUpper();
                        if (resposta != "S" && resposta != "N")
                        {
                            Console.WriteLine("\nresponda com 'S' ou 'N': ");
                            continue;
                        }
                        if (resposta == "N")
                        {
                            DesfazerUltimaAcao(_rollback);
                            Console.WriteLine("Desfazendo a última ação...");
                        }
                        else if (resposta == "S")
                        {
                            _listaDeDispositivosAtivos.Remove(dispositivo);
                            bloqueados.Add(dispositivo);
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Bloqueado: {dispositivo.Nome} ({dispositivo.Fabricante})");
                            Console.ResetColor();
                        }
                        break;
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Liberado: {dispositivo.Nome} ({dispositivo.Fabricante})");
                    Console.ResetColor();
                }

            }

            Console.WriteLine("__________________________________");
            Console.WriteLine($"Total bloqueado: {bloqueados.Count} | Total liberado: {_listaDeDispositivosAtivos.Count}\n");
        }

        public IEnumerable<Dispositivo> ObterFilaDeAutenticacao()
        {
            return _filaDeAutenticacaoDeDispositivos;
        }

        public IEnumerable<Dispositivo> ObterDispositivosAtivos()
        {
            return _listaDeDispositivosAtivos;
        }
    }
}
