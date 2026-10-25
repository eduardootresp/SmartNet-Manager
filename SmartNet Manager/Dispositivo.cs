using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartNet_Manager
{
    public class Dispositivo
    {
        [Required]
        public string MacAddress {  get; set; }
        [Required]
        public string EnderecoIP { get; set; }
        [Required]
        public string Nome {  get; set; }
        [Required]
        public string Fabricante { get; set; }
        [Required]
        public double ConsumoBanda { get; set; }
        public DateTime DataConexao { get; set; } = DateTime.Now;

        public Dispositivo(string macAddress, string enderecoIP, string nome, string fabricante,
            double consumoBanda) 
        { 
            MacAddress = macAddress;
            EnderecoIP = enderecoIP;
            Nome = nome;
            Fabricante = fabricante;
            ConsumoBanda = consumoBanda;
        }

        public override string ToString()
        {
            return $"{Nome} | MAC: {MacAddress} | IP: {EnderecoIP} | Fabricante: {Fabricante} | Consumo: {ConsumoBanda} Mbps | Conectado: {DataConexao:yyyy-MM-dd HH:mm}";
        }
    }
}
