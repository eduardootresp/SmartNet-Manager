using System.Collections;
using System.Linq;
using System.Net.WebSockets;

namespace SmartNet_Manager
{
    internal class Program
    {
        static Repository repositorio;
        static DhcpRepository dhcpRepositorio;
        static void Main(string[] args)
        {
            object instanciaObjeto = Activator.CreateInstance(typeof(Repository));
            repositorio = (Repository)instanciaObjeto;

            object instanciaObjeto_2 = Activator.CreateInstance(typeof(DhcpRepository));
            dhcpRepositorio = (DhcpRepository)instanciaObjeto_2;

            var authRepo = new Filas();
            var pilhaLog = new Filas();
            bool executando = true;

            while (executando)
            {
                Console.WriteLine("\n=== SERVIDOR DE REDE ===");
                Console.WriteLine("1 Menu - Criar, Editar novos dispositivos, Fazer reserva");
                Console.WriteLine("2 Adicionar Todos os dispositivos do repositório a fila");
                Console.WriteLine("3. Processar fila (Um por vez)");
                Console.WriteLine("4. Ver fila de autenticação");
                Console.WriteLine("5. Ver dispositivos ativos");
                Console.WriteLine("6. Bloquear dispositivos");
                Console.WriteLine("7. Exibir Dashboard");
                Console.WriteLine("8. Imprimir Relatório");
                Console.WriteLine("0. Sair");
                Console.Write("\nEscolha uma opção: ");

                string opcao = Console.ReadLine();

                switch (opcao)
                {
                    case "1":
                        MenuRepositorio();
                        break;
                    case "2":
                        var todoRepositorio = repositorio.GetAll();
                        foreach (var todoRepo in todoRepositorio)
                        {
                            authRepo.AdicionarDispositivoAFila(todoRepo);
                        }
                        break;
                    case "3":
                        authRepo.ProcessarFila();
                        break;
                    case "4":
                        authRepo.MostrarFila();
                        break;
                    case "5":
                        authRepo.MostrarAtivos();
                        break;
                    case "6":
                        Console.WriteLine("\n=== MEU DE BLOQUEIOS ===");
                        Console.WriteLine("Digite a regra do bloqueio: ");
                        Console.WriteLine("1. Bloquear aparelhos 'Apple'");
                        Console.WriteLine("2. Bloquear aparelhos 'Samsung");
                        Console.WriteLine("3. Bloquear aparelhos 'Dell");
                        Console.WriteLine("4. Bloquear aparelho com consumo acima de '500mb'");
                        Console.WriteLine("5. Criar regra nova: ");
                        Console.WriteLine("0. Voltar");
                        authRepo.MostrarAtivos();
                        Console.Write("\nEscolha uma opção: ");

                        var regraBloqueio = Console.ReadLine();

                        Func<Dispositivo, bool> filtro = d => false;
                        bool aplicar = true;

                        switch (regraBloqueio)
                        {
                            case "1":
                                filtro = d => d.Fabricante == "Apple";
                                break;
                            case "2":
                                filtro = d => d.Fabricante == "Samsung";
                                break;
                            case "3":
                                filtro = d => d.Fabricante == "Dell";
                                break;
                            case "4":
                                filtro = d => d.ConsumoBanda > 500;
                                break;
                            case "5":
                                Console.WriteLine("\n=== OPCOES  ===");
                                Console.WriteLine("1. Nova Regra para fabricante (nome do fabricante)");
                                Console.WriteLine("2. Nova Regra para consumo (valor mínimo em mb que quer bloquear)");
                                Console.Write("\nEscolha uma opção: ");
                                var menuNovaRegra = Console.ReadLine();
                                switch (menuNovaRegra)
                                {
                                    case "1":
                                        Console.WriteLine("Digite o nome do fabricante a ser bloqueado: ");
                                        var novaRegraFab = Console.ReadLine();
                                        filtro = d => d.Fabricante == novaRegraFab;
                                        break;
                                    case "2":
                                        Console.WriteLine("Digite o valor mínimo de consumo a ser bloqueado: ");
                                        if (double.TryParse(Console.ReadLine(), out double novaRegraCon))
                                        {
                                            filtro = d => d.ConsumoBanda > novaRegraCon;
                                        }
                                        else
                                        {
                                            Console.WriteLine("Valor inválido para consumo.");
                                            aplicar = false;
                                        }
                                        break;
                                    default:
                                        Console.WriteLine("Opção inválida.");
                                        aplicar = false;
                                        break;
                                }
                                break;
                            case "0":
                                Console.WriteLine("voltando...");
                                break;
                            default:
                                Console.WriteLine("Regra desconhecida. Nenhum dispositivo bloqueado");
                                aplicar = false;
                                break;
                        }
                        if (aplicar)
                        {
                            authRepo.AplicarFirewall(filtro);
                            Console.WriteLine("Regra de bloqueio aplicada com sucesso.");
                        }
                        break;           
                    case "7":
                        authRepo.ExibirDashBoard();
                        break;
                    case "8":
                        var filaDeAutenticados = authRepo.ObterFilaDeAutenticacao();
                        var filaDispositivosAtivos = authRepo.ObterDispositivosAtivos();
                        bool executandoSubMenu = true;

                        while (executandoSubMenu)
                        {
                            Console.WriteLine("\n=== MENU RELATÓRIO ===");
                            Console.WriteLine("1. Ver relatório da Fila de Autenticação");
                            Console.WriteLine("2. Ver relatório da Lista de Dispositivos Ativos");
                            Console.WriteLine("0. Voltar");
                            Console.Write("\nEscolha uma opção: ");

                            string opcaoSubmenu = Console.ReadLine();

                            switch (opcaoSubmenu)
                            {
                                case "1":
                                    Console.WriteLine("Ver relatório da Fila de Autenticação");
                                    authRepo.ImprimirRelatorio(filaDeAutenticados);
                                    break;
                                case "2":
                                    Console.WriteLine("Ver relatório da Lista de Dispositivos Ativos");
                                    authRepo.ImprimirRelatorio(filaDispositivosAtivos);
                                    break;
                                case "0":
                                    executandoSubMenu = false;
                                    Console.WriteLine("Encerrando submenu...");
                                    break;
                                default:
                                    Console.WriteLine("Opção inválida!");
                                    break;
                            }
                        }
                        break;
                    case "0":
                        executando = false;
                        Console.WriteLine("Encerrando servidor...");
                        break;
                    default:
                        Console.WriteLine("❌ Opção inválida!");
                        break;
                }
            }
        }

        public static void MenuRepositorio()
        {

            bool executando = true;
            while (executando)
            {
                Console.WriteLine("\n=== MENU REPOSITORIO ===");
                Console.WriteLine("1. Criar novo Dispositivo para o repositório");
                Console.WriteLine("2. Listar todos dispositivos do repositório");
                Console.WriteLine("3. Encontrar IP pelo MAC do aparelho atual");
                Console.WriteLine("4. Editar Ip do aparelho");
                Console.WriteLine("5. Fazer reserva do MAC/IP. (DhcpRepository)");
                Console.WriteLine("6. Obter IP pelo MAC (DhcpRepository)");
                Console.WriteLine("7. Listar reserva (DhcpRepository)");
                Console.WriteLine("0. Sair");
                Console.Write("\nEscolha uma opção: ");

                string opcao = Console.ReadLine();

                switch (opcao)
                {
                    case "1":
                        Console.Write("Digite o MAC Address: Ex: \"AA:BB:CC:11:22\": ");
                        string mac = Console.ReadLine();
                        Console.Write("Digite o Endereço IP: Ex: \"192.168.0.10\": ");
                        string ip = Console.ReadLine();
                        Console.Write("Digite o Nome: Ex: \"PC-Contabilidade\": ");
                        string nome = Console.ReadLine();
                        Console.Write("Digite o Fabricante: Ex: \"Dell\", \"Apple\", \"Samsung\": ");
                        string fabricante = Console.ReadLine();
                        Console.Write("Digite o Consumo de Banda: // Em Mbps: ");
                        double consumoBanda = double.TryParse(Console.ReadLine(), out double cb) ? cb : 0;

                        Dispositivo novoDispositivo = new Dispositivo(mac, ip, nome, fabricante, consumoBanda);
                        repositorio.AddOrUpdate(novoDispositivo);

                        Console.Write("Deseja adicionar este dispositivo a Reserva de IP's? (S/N)");
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
                                Console.WriteLine("O dispositivo ficará apenas no repositório");
                            }
                            else if (resposta == "S")
                            {
                                string mensagemReserva;
                                dhcpRepositorio.AdicionarReserva(novoDispositivo.MacAddress, novoDispositivo.EnderecoIP, out mensagemReserva);
                                Console.WriteLine(mensagemReserva);
                                Console.WriteLine("O dispositivo foi reservado com sucesso");
                            }
                            break;
                        }
                        break;
                    case "2":
                        Console.WriteLine("\n=== Dispositivos no Repositório ===");
                        foreach (var dispositivo in repositorio.GetAll())
                        {
                            Console.WriteLine(dispositivo.ToString());
                        }
                        break;
                    case "3":
                        Console.Write("Digite o MAC Address: Ex: \"AA:BB:CC:11:22\": ");
                        string getByMac = Console.ReadLine();
                        repositorio.GetByMacAddress(getByMac);
                        break;
                    case "4":
                        Console.Write("Digite o MAC Address: Ex: \"AA:BB:CC:11:22\": ");
                        string editByMac = Console.ReadLine();
                        repositorio.EditByMacAddress(editByMac);
                        break;
                    case "5":
                        Console.Write("Digite o MAC Address: Ex: \"AA:BB:CC:11:22\": ");
                        string reservaMac = Console.ReadLine();
                        Console.Write("Digite o Endereço IP: Ex: \"192.168.0.10\": ");
                        string reservaIp = Console.ReadLine();
                        string mensagem;
                        dhcpRepositorio.AdicionarReserva(reservaMac, reservaIp, out mensagem);
                        Console.WriteLine(mensagem);
                        break;
                    case "6":
                        Console.Write("Digite o MAC Address: Ex: \"AA:BB:CC:11:22\": ");
                        string pesquisaMac = Console.ReadLine();
                        dhcpRepositorio.ObterIP(pesquisaMac);
                        break;
                    case "7":
                        Console.WriteLine("\n=== Reservas no DHCP ===");
                        dhcpRepositorio.ListarReserva();
                        break;
                    case "0":
                        executando = false;
                        Console.WriteLine("Voltando ao Menu Principal...");
                        break;
                    default:
                        Console.WriteLine("Opção inválida!");
                        break;
                }
            }
        }
    }

}

