var t=document.querySelector("#app");if(!t)throw Error("App root was not found.");t.innerHTML=`
  <main class="page">
    <section class="hero">
      <p class="lead">
        The some page
      </p>
    </section>


    <footer class="footer">
      <span><strong>Build target</strong></span>
      <span id="year"></span>
    </footer>
  </main>
`;var e=document.querySelector("#year");if(e)e.textContent=new Date().getFullYear().toString();

//# debugId=5540EE989ED27A2064756E2164756E21
//# sourceMappingURL=main.js.map
