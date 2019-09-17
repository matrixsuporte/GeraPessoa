using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using HtmlAgilityPack;
using Indigosoft.RobotsMethods.Libraries.HtmlAgilityPackExtensions;
using Indigosoft.RobotsMethods.Libraries.HtmlAgilityPackExtensions.Helpers;
using System.Net;
using WebApplication1.Models; // usei para poder usar o meu modelo
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System;
using System.Web;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Web.Script.Serialization;

namespace WebApplication1.Controllers
{
    public class PessoaController : Controller
    {
        public BrowserSession _web;
        public static Dictionary<string, string> listCidades = new Dictionary<string, string>();

        // GET: Pessoa
        public ActionResult Index()
        {
            //List<Pessoa> lista_de_Pessoas = new List<Pessoa>();
            List<Pessoa> lista_de_Estados = new List<Pessoa>();
            List<Pessoa> lista_de_Cidades = new List<Pessoa>();

            Iniciar();
            //baixar os dados do site que desejo obter as informações
            string pagina = _web.Get("gerador_de_pessoas");

            //criando uma instancia chamada documento_html
            var documento_html = new HtmlAgilityPack.HtmlDocument();

            //jogando os dados dentro do documento HTML
            documento_html.LoadHtml(pagina);

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // coleção de dados genericos, no caso as idades do combo idade
            //validar esta perte, pois se nao tiver internet da erro
            HtmlNodeCollection nodeIdades = documento_html.GetElementById("idade").ChildNodes;

            List<string> lista_Idades = new List<string>();

            foreach (HtmlNode nodeIdade in nodeIdades)
            {
                if (nodeIdade.InnerText != "" && !nodeIdade.InnerText.Contains("\n"))
                {
                    lista_Idades.Add(nodeIdade.InnerText);
                }
            }
            ViewBag.Seleciona_Idade = new SelectList(lista_Idades);

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////            
            // coleção de dados genericos, no caso os estados do combo estado
            HtmlNodeCollection nodeEstados = documento_html.GetElementbyId("cep_estado").ChildNodes;

            List<string> lista_Estados = new List<string>();

            //listar o nodeEstado e so add os itens se  for diferente de vazio e diferente da quebra de linha com estes espaços
            foreach (HtmlNode nodeEstado in nodeEstados)
            {
                if (nodeEstado.InnerText != "" && !nodeEstado.InnerText.Contains("\n"))
                {
                    //cmbEstadoPesquisa.Items.Add(nodeEstado.InnerText);
                    lista_Estados.Add(nodeEstado.InnerText);
                }
            }
            //ViewBag.mesmo nome que vai aparecer no HTML
            ViewBag.Estados = new SelectList(lista_Estados);

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            var lista_cidades = new List<string>();
            lista_cidades.Add("Selecione");
            ViewBag.Cidades = new SelectList(lista_cidades);

            return View();
        }

        private List<string> CarregarCidades(string uf = "SP")
        {
            //iniciar a a variavel pagina vazia para receber os dados do webclient, parecido com HtmlAgilityPack
            string pagina = string.Empty;
            List<string> lista_cidades = new List<string>();

            //WebClient - Ele fornece métodos para enviar dados ou receber dados de qualquer recurso identificado pela URL
            // acabei usando isso porque não consegui reaproveitar o codigo anterior
            using (WebClient client = new WebClient())
            {
                //converter os dados das cidades para utf8
                client.Encoding = Encoding.UTF8; // UTF8.GetString(client);
                var parametrosPostCidades = new NameValueCollection();

                //Get que acontece quando mando carregar as cidades do estado, só preciso converter os dados depois para UTF8
                parametrosPostCidades["acao"] = "carregar_cidades";
                parametrosPostCidades["cep_estado"] = uf;

                byte[] respBytes = client.UploadValues("https://www.4devs.com.br/ferramentas_online.php", parametrosPostCidades);
                string response = client.Encoding.GetString(respBytes);
                pagina = response; // o responde será salvo na variavel pagina pra eu poder usar depois e não perder neste bloco
            }

            //criando uma instancia chamada documento_html
            var documento_html = new HtmlAgilityPack.HtmlDocument();

            //jogando os dados dentro do documento HTML
            documento_html.LoadHtml(pagina);

            // coleção de dados genericos, no caso as cidades do combo cidade
            ArrayList cidades = new ArrayList();
            HtmlNodeCollection nodeCidades = documento_html.DocumentNode.ChildNodes;

            string codCidade = string.Empty;
            listCidades = new Dictionary<string, string>();
            foreach (HtmlNode nodeCidade in nodeCidades)
            {
                //if (nodeCidade.InnerText != "" && nodeCidade.InnerText != "\n                        ")

                //!nodeCidade.InnerText.Contains("\n")
                if (nodeCidade.InnerText != "" && !nodeCidade.InnerText.Contains("\n"))

                {
                    lista_cidades.Add(nodeCidade.InnerText);
                    listCidades.Add(codCidade, nodeCidade.InnerText);

                    codCidade = string.Empty;
                }
                //caso contenha a tag chamada VALUE dentro do HTML ele
                else if (nodeCidade.OuterHtml.Contains("value"))
                {
                    codCidade = nodeCidade.OuterHtml.GetStringBetweenCharacters('\"');
                }
            }

            return lista_cidades;

        }

        public void Iniciar()
        {
            var helper = new HttpWebResponseHelper("https://www.4devs.com.br/")
            {
                CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore),
                Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3",
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.86 Safari/537.36",
                KeepAlive = true,
                AutomaticDecompression = DecompressionMethods.GZip,
            };

            helper.Headers.Add("Accept-Language: pt-BR,pt;q=0.9,en-US;q=0.8,en;q=0.7");
            helper.Headers.Add("Accept-Encoding: gzip, deflate, br");

            _web = new BrowserSession(helper, helper);
        }

        // GET: Pessoa/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Pessoa/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Pessoa/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Pessoa/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Pessoa/BuscarCidades/sp
        [HttpPost]
        public JsonResult BuscarCidades(string ufSelecionado)
        {
            var lista_cidades = CarregarCidades(ufSelecionado);

            return Json(lista_cidades.ToList());
        }



        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Gerar Pessoa
        [HttpPost]
        public JsonResult GerarPessoa(string sexo, string geraPonto, string gerarIdade, string uf, string cidade)
        {
            var pessoa = gerarPessoaCrawler(sexo, geraPonto, gerarIdade, uf, cidade);
            return Json(pessoa);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



        private Pessoa gerarPessoaCrawler(string sexo, string geraPonto, string gerarIdade, string uf, string cidade)
        {
            string responseGerarPessoa = string.Empty;

            var listNumber = listCidades.Keys.ToList();

            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8; // UTF8.GetString(client);
                var parametrosPostGerarPessoa = new NameValueCollection();

                //paramentro da classe pessoa       [tipo da ação]  = variavel;
                parametrosPostGerarPessoa["acao"] = "gerar_pessoa";
                parametrosPostGerarPessoa["sexo"] = sexo;
                parametrosPostGerarPessoa["pontuacao"] = geraPonto;
                parametrosPostGerarPessoa["idade"] = gerarIdade;
                parametrosPostGerarPessoa["cep_estado"] = uf;
                parametrosPostGerarPessoa["txt_qtde"] = "1";
                parametrosPostGerarPessoa["cep_cidade"] = listCidades.FirstOrDefault(x => x.Value.Contains(cidade)).Key;

                // pego a resposta do site para gerar o objeto pessoa. Tive que fazer novamente  o downlod porque não consegui usar o codigo anterior
                byte[] respBytes = client.UploadValues("https://www.4devs.com.br/ferramentas_online.php", parametrosPostGerarPessoa);
                string response = client.Encoding.GetString(respBytes);
                responseGerarPessoa = response;

                Pessoa objPessoa = JsonConvert.DeserializeObject<Pessoa>(response);
                objPessoa.Sexo = sexo;
                return objPessoa;
            }
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}