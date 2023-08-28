<h1>CachedAttribute para ASP.NET Core</h1>
<h3>Descrição</h3>
<h2>Descrição</h2>
<p>O <code>CachedAttribute</code> é um atributo personalizado para ASP.NET Core que permite armazenar o resultado bem-sucedido de uma ação no cache.
  Ele utiliza o <code>MemoryCache</code> para armazenar os resultados e reutilizá-los em solicitações futuras, 
  reduzindo a necessidade de recalculações e melhorando a eficiência.</p>

<br>
<h2>Características</h2>
<ul>
    <li>Armazena o resultado de ações bem-sucedidas no cache com uma chave específica e um tempo de expiração.</li>
    <li>Reutiliza o resultado armazenado para solicitações futuras, desde que o cache ainda não tenha expirado.</li>
    <li>Limpa o cache automaticamente quando uma requisição que não seja <code>GET</code> é feita, garantindo que os dados sejam atualizados.</li>
</ul>

<h2>Como usar</h2>
<p>1. Adicione o atributo <code>[Cached]</code> à classe ou método que você deseja armazenar em cache.</p>
<p>2. Opcionalmente, você pode fornecer um tempo de expiração em minutos. Por padrão, é 5 minutos.</p>
<pre><code>
[Cached(10)] // Cache por 10 minutos
public IActionResult MinhaAcao()
{
    // Seu código aqui
}
</code></pre>

<h2>Considerações</h2>
<ul>
    <li>O cache é armazenado na memória, o que significa que ele será limpo se o aplicativo for reiniciado.</li>
    <li>O atributo foi projetado principalmente para ações que retornam um <code>OkObjectResult</code>. Outros tipos de resultados não serão armazenados em cache.</li>
</ul>


