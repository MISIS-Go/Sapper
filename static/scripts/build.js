import { copyFile, mkdir, rm } from "node:fs/promises";
import { dirname, join } from "node:path";
import { fileURLToPath } from "node:url";

const rootDir = dirname(dirname(fileURLToPath(import.meta.url)));
const srcDir = join(rootDir, "src");
const distDir = join(rootDir, "dist");

await rm(distDir, { recursive: true, force: true });

const result = await Bun.build({
  entrypoints: [join(srcDir, "main.ts")],
  outdir: distDir,
  target: "browser",
  minify: true,
  sourcemap: "linked",
});

if (!result.success) {
  for (const log of result.logs) {
    console.error(log);
  }

  process.exit(1);
}

await mkdir(distDir, { recursive: true });
await copyFile(join(srcDir, "index.html"), join(distDir, "index.html"));
