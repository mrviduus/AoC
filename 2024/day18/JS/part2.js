const fs = require("fs");
const path = require("path");
const INPUT_FILE = path.join(__dirname, "../input.txt");

fs.readFile(INPUT_FILE, "utf-8", (err, input) => {
  if (err) {
    console.log("Error reading file:", err);
    process.exit(1);
  }

  const inputLines = input.trim().split("\n");
  const size = 70;

  // Инициализируем пустую карту: 0 — безопасная ячейка
  const map = Array.from({ length: size + 1 }, () => new Array(size + 1).fill(0));

  // Проверка, существует ли путь от (0,0) до (70,70)
  function pathExists(map) {
    const queue = [[0, 0]];
    const visited = Array.from({ length: size + 1 }, () => Array(size + 1).fill(false));
    visited[0][0] = true;

    const directions = [
      [0, 1], [0, -1],
      [1, 0], [-1, 0]
    ];

    while (queue.length > 0) {
      const [x, y] = queue.shift();

      if (x === size && y === size) return true;

      for (let [dx, dy] of directions) {
        const nx = x + dx;
        const ny = y + dy;

        if (
          nx >= 0 && nx <= size &&
          ny >= 0 && ny <= size &&
          !visited[ny][nx] &&
          map[ny][nx] === 0
        ) {
          visited[ny][nx] = true;
          queue.push([nx, ny]);
        }
      }
    }

    return false;
  }

  // Постепенно добавляем байты и проверяем путь
  for (let i = 0; i < inputLines.length; i++) {
    const [x, y] = inputLines[i].split(",").map(Number);
    map[y][x] = 1; // порченая ячейка

    if (!pathExists(map)) {
      console.log(`${x},${y}`); // формат без лишних символов
      return;
    }
  }

  console.log("Путь никогда не был заблокирован.");
});