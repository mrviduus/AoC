const fs = require("fs");
const path = require("path");
const INPUT_FILE = path.join(__dirname, "../input.txt");

fs.readFile(INPUT_FILE, "utf-8", (err, input) => {
  if(err){
    console.log("Error reading file", err);
    process.exit(1);
  }
  // Разбиваем весь текст карты на массив строк (каждая строка = один ряд лабиринта)
  let gridLines = input.split("\n");
  // Высота карты (сколько строк)
  let H = gridLines.length;

  // Ширина карты (сколько символов в строке)
  // Предполагается, что все строки одинаковой длины
  let W = gridLines[0].length;

  // Поможем себе: преобразуем карту в удобный двумерный массив символов
  const grid = gridLines.map(lines => lines.split(""));


  // Найдём координаты старта S и финиша E
  let start = null; //Сюда пишем [x, y] старта
  let end = null; // Сюда пишем [x, y] финиша

  for (let y = 0; y < H; y++) {              // идём по всем строкам
    for (let x = 0; x < W; x++) {            // идём по всем колонкам
      if (grid[y][x] === 'S') {              // нашли символ 'S' ?
        start = [x, y];                      // запоминаем координату старта
      }
      if (grid[y][x] === 'E') {              // нашли символ 'E' ?
        end = [x, y];                        // запоминаем координату финиша
      }
    }
  }


  // ================== BFS (ПОИСК КРАТЧАЙШЕГО ПУТИ БЕЗ ЧИТА) ==================

  // Нам нужно знать:
  // distFromStart[y][x] = за сколько шагов можно честно дойти от S до клетки (x,y)
  // distFromEnd[y][x]   = за сколько шагов можно честно дойти от этой клетки (x,y) до E
  // Это обычный BFS по лабиринту, ходим только по не-стенам (# это стена).
  function bfs(sourceX, sourceY) {
    //Создаем матрицу расстояний, изначально все -1 (тоесть недостижимо)
    const dist = Array.from({length: H }, () => Array(W).fill(-1));
    //Очередь для BFS
    const queue = [];

    //Стартовая клетка достижима за 0 шагов
    dist[sourceY][sourceX] = 0;
    queue.push([sourceX, sourceY]);

    //Пока очередь не пуста
    while(queue.length > 0){
      const [x, y] = queue.shift(); //берем первую позицию

      //пробуем пройти во все 4 стороны

      const dirs = [
        [1, 0],
        [-1, 0],
        [0, 1],
        [0, -1]
      ];

      for(const [dx, dy] of dirs){
        const nx = x + dx; //new x
        const ny = y + dy; //new y

        if(nx < 0 || nx >= W || ny < 0 || ny >= H) continue;
        // Проверяем что это не стена
        if(grid[ny][nx] === '#') continue;
        
        // И что мы туда еще не ходили
        if(dist[ny][nx] !== -1) continue;
        
        // Тогда расстояние до новой клетки = расстояние до текущей + 1 шаг
        
        dist[ny][nx] = dist[y][x] + 1;

        //Кладем первую клетку в очередь
        queue.push([nx, ny]);
      }
    }
    return dist; //возвращаем матрицу расстояний

  }
  // Считаем растояние от старта до финиша
  const distFromStart = bfs(start[0], start[1]);
  const distFromEnd = bfs(end[0], end[1]);

  //Честное минимальное расстояние
  const baseTime = distFromStart[end[1]] [end[0]];
  // Это то самое "84" из маленького примера, но для твоей большой карты — своё число (у меня получилось 9444, см. расчёт выше на основе файла).  [oai_citation:1‡input.txt](sediment://file_00000000109861f7bb530acf6876be6c)


  // ================== ПРАВИЛО ЧИТА ==================
  //
  // 1 раз за гонку можно "включить призрак" максимум на 2 шага.
  // Это значит: ты можешь взять ЛЮБУЮ клетку трека A (где grid != '#'),
  // и ЛЮБУЮ клетку трека B (где grid != '#'),
  // если манхэттенское расстояние между ними (|ax-bx| + |ay-by|) равно 1 или 2,
  // то ты МОЖЕШЬ представить, что прыгнул из A в B НАПРЯМУЮ за это расстояние,
  // даже если между ними стены.
  //
  // После этого ты продолжаешь бежать честно.
  //
  // Время маршрута с читом типа "A -> B" будет:
  // distFromStart[A] + (манхэттен(A,B)) + distFromEnd[B]
  //
  // Экономия = baseTime - этоВремя.
  //
  // Мы считаем ТОЛЬКО если экономия > 0 (чит реально ускоряет) и экономия >= 100.
  //
  // Важно: каждая пара (A,B) считается отдельным читом,
  // даже если экономия одинаковая. Это прямо из условия.
  
  let countCheatsAtLeast100 = 0; // сюда посчитаем читов с экономией >= 100.

  //Пройдем по всем клеткам
  for(let ay = 0; ay < H; ay++){
    for(let ax = 0; ax < W; ax++){
      
      if(grid[ay][ax] === '#') continue;
      const dStartA = distFromStart[ay][ax];
      if (dStartA === -1) continue;

      for(let dy = -2; dy <= 2; dy++){
        for(let dx = -2; dx <= 2; dx++){
          const manhattan = Math.abs(dx) + Math.abs(dy);
          if(manhattan !== 1 && manhattan !== 2){
            continue;
          }

          const bx = ax + dx;
          const by = ay + dy;

          if(bx < 0 || bx >= W || by < 0 || by >= H) continue;

          if(grid[by][bx] === '#') continue;
          
          const dEndB = distFromEnd[by][bx];
          if(dEndB === -1) continue;
          const cheatTime = dStartA + manhattan + dEndB;

          const saving = baseTime - cheatTime;

          if(saving >= 100){
            countCheatsAtLeast100++;
          }
        }
      }
    }
  }

  console.log(countCheatsAtLeast100);

});
