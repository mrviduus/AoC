const fs = require("fs");
const path = require("path");

const INPUT_FILE = path.join(__dirname, "../input.txt");

fs.readFile(INPUT_FILE, "utf-8", (err, input) => {
  if (err) {
    console.error("Ошибка чтения файла:", err);
    process.exit(1);
  }

  const lines = input.split(/\r?\n/).map(line => line.trim()).filter(Boolean);

  // Извлекаем программу
  const programLine = lines.find(line => line.startsWith("Program:"));
  const originalProgram = programLine.split(":")[1].trim().split(",").map(Number);

  // Функция, которая запускает программу с конкретным значением регистра A
  const runProgram = (initialA, program) => {
    let registers = [initialA, 0, 0];
    let ip = 0;
    const outputs = [];

    while (ip < program.length && outputs.length < program.length) {
      const opcode = program[ip];
      if (ip + 1 >= program.length) break;
      const operand = program[ip + 1];

      switch (opcode) {
        case 0:
          registers[0] = Math.floor(registers[0] / (2 ** (operand <= 3 ? operand : registers[operand - 4])));
          break;
        case 1:
          registers[1] ^= operand;
          break;
        case 2:
          registers[1] = (operand <= 3 ? operand : registers[operand - 4]) % 8;
          break;
        case 3:
          if (registers[0] !== 0) {
            ip = operand;
            continue;
          }
          break;
        case 4:
          registers[1] ^= registers[2];
          break;
        case 5:
          outputs.push((operand <= 3 ? operand : registers[operand - 4]) % 8);
          // Проверка, совпадает ли текущий вывод с программой
          if(outputs[outputs.length - 1] !== program[outputs.length - 1]){
            return false; // если не совпадает, сразу выходим
          }
          break;
        case 6:
          registers[1] = Math.floor(registers[0] / (2 ** (operand <= 3 ? operand : registers[operand - 4])));
          break;
        case 7:
          registers[2] = Math.floor(registers[0] / (2 ** (operand <= 3 ? operand : registers[operand - 4])));
          break;
      }
      ip += 2;
    }

    // Если длина вывода совпадает с длиной программы, то нашли число
    return outputs.length === program.length;
  };

  // Ищем минимальное число для A
  let minimalA = 1;
  while (true) {
    if (runProgram(minimalA, originalProgram)) {
      console.log("Минимальное значение для регистра A:", minimalA);
      break;
    }
    minimalA += 1;
  }
});
