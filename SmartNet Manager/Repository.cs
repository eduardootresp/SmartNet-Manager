using SmartNet_Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SmartNet_Manager
{
    internal class Repository
    {
        private Dictionary<string, Dispositivo> TabelaDeDispositivos { get; set; } = new Dictionary<string, Dispositivo>

        {
            {
                "AA:BB:CC:11:22", // CHAVE 
                new Dispositivo("AA:BB:CC:11:22","192.168.0.10","PC-Contabilidade","Dell",100) // VALOR 
            },
            
            {
                "AA:BB:CC:11:23",
                new Dispositivo("AA:BB:CC:11:23","192.168.0.11","PC-Marketing","Samsung",200)   // Fabricantes permitidos "Apple", "Samsumg", "Dell"
            },
            {
                "AA:BB:CC:11:24", 
                new Dispositivo("AA:BB:CC:11:24","192.168.0.12","PC-Rh","Apple",600)   
            },
            {
                "AA:BB:CC:11:25",
                new Dispositivo("AA:BB:CC:11:25","192.168.0.13","PC-Diretoria","LG",150)   
            }
        };
        public void AddOrUpdate(Dispositivo dispositivo)
        {
            if (dispositivo == null) throw new ArgumentNullException(nameof(dispositivo));
            // acesso direto
            if (string.IsNullOrWhiteSpace(dispositivo.MacAddress))
                throw new ArgumentException("O dispositivo deve possuir um endereço MAC válido para ser usado como chave.", nameof(dispositivo));
            if (TabelaDeDispositivos.ContainsKey(dispositivo.MacAddress))
                throw new ArgumentException("O dispositivo não pode possuir o mesmo endereço MAC de outro dispositivo.", nameof(dispositivo));

            // acesso direto
            TabelaDeDispositivos[dispositivo.MacAddress] = dispositivo;
        }

        public Dispositivo? GetByMacAddress(string mac)
        {
            GarantirRepositorio();

            if (string.IsNullOrWhiteSpace(mac)) throw new ArgumentException("MAC Address não pode ser vazio.", nameof(mac));

            if (TabelaDeDispositivos.TryGetValue(mac, out Dispositivo dispositivo))
            {
                Console.WriteLine($"O ip do dispositivo {dispositivo.Nome} é {dispositivo.EnderecoIP}");
                return dispositivo;
            }

            return null;
        }

        public Dispositivo EditByMacAddress(string mac)
        {
            GarantirRepositorio();
            if (string.IsNullOrWhiteSpace(mac))
                throw new ArgumentException("MAC Address não pode ser vazio.", nameof(mac));
            Console.WriteLine("Digite o novo IP do aparelho: ");
            var novoIp = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(novoIp))
                throw new ArgumentException("Novo IP não pode ser vazio.", nameof(novoIp));
            
            var dispositivoEncontrado = GetByMacAddress(mac);
            dispositivoEncontrado.EnderecoIP = novoIp;

            Console.WriteLine($"O novo endereço de ip agora é {novoIp}");
            return dispositivoEncontrado;
        }

        public IEnumerable<Dispositivo> GetAll()
        {
            GarantirRepositorio();
            return TabelaDeDispositivos.Values.ToList();
        }

        private void GarantirRepositorio()
        {
            if (TabelaDeDispositivos == null)
            {
                TabelaDeDispositivos = new Dictionary<string, Dispositivo>(StringComparer.OrdinalIgnoreCase);
            }
        }
    }
}
