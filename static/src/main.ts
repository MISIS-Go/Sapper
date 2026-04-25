import "./styles.css";

const app = document.querySelector<HTMLDivElement>("#app");

if (!app) {
  throw new Error("App root was not found.");
}

app.innerHTML = `
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
`;

const year = document.querySelector<HTMLSpanElement>("#year");

if (year) {
  year.textContent = new Date().getFullYear().toString();
}
