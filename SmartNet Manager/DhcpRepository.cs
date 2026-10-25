using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartNet_Manager
{
    internal class DhcpRepository
    {
        // Chave: MacAddress, Valor: EnderecoIP
        private Dictionary<string, string> ReservaIPs { get; set; } = new Dictionary<string, string>();

        public bool AdicionarReserva(string macAddress, string enderecoIP, out string mensagem)
        {
            mensagem = string.Empty;

            macAddress = macAddress?.Trim() ?? string.Empty;
            enderecoIP = enderecoIP?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(macAddress))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                mensagem = "O MAC Address não pode ser vazio.";
                Console.ResetColor();
                return false;
            }

            if (string.IsNullOrWhiteSpace(enderecoIP))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                mensagem = "O Endereço IP não pode ser vazio.";
                Console.ResetColor();
                return false;
            }

            if (ReservaIPs.ContainsKey(macAddress))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ATENÇÃO!!");
                mensagem = $"Não foi possível adicionar, pois já existe uma reserva para o MAC: {macAddress}";
                Console.ResetColor();
                return false;
            }

            if (ReservaIPs.ContainsValue(enderecoIP))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ATENÇÃO!!");
                mensagem = $"Não foi possível adicionar, pois já existe uma reserva para o IP: {enderecoIP}";
                Console.ResetColor();
                return false;
            }

            // Se passou por todas as validações, adiciona com sucesso
            ReservaIPs[macAddress] = enderecoIP;
            mensagem = "Reserva de IP adicionada com sucesso!";
            return true;
        }

        public string ObterIP(string macAddress)
        {
            string buscaMac = macAddress?.Trim() ?? string.Empty;

            if (ReservaIPs.TryGetValue(buscaMac, out string ip))
            {
                Console.WriteLine($"MAC: {macAddress} --> IP: {ip}");
                return ip;
            }
            return "(Vazio)";
        }

        public void ListarReserva()
        {
            foreach (var item in ReservaIPs)
            {
                Console.WriteLine($"MAC: {item.Key} --> IP: {item.Value}");
            }
        }

    }
}
