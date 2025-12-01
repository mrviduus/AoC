const fs = require("fs");
const path = require("path");

const INPUT_FILE = path.join(__dirname, "../input.txt");

fs.readFile(INPUT_FILE, "utf-8", (err, input) => {
  if (err) {
    console.error("Error reading file:", err);
    process.exit(1);
  }

  // Normalize line endings and remove empty lines explicitly
  const lines = input.split(/\r?\n/).map(line => line.trim()).filter(line => line !== "");

  // For debugging clarity, log parsed lines
  console.log("Parsed Lines:", lines);

  if (lines.length < 4) {
    console.error("Input file format error: expected at least 4 lines");
    process.exit(1);
  }

  // Extract registers explicitly
  const A = parseInt(lines[0].split(":")[1].trim(), 10);
  const B = parseInt(lines[1].split(":")[1].trim(), 10);
  const C = parseInt(lines[2].split(":")[1].trim(), 10);

  // Ensure program line is correctly identified and parsed
  const programLine = lines.find(line => line.startsWith("Program:"));
  if (!programLine) {
    console.error("Program line not found in the input file");
    process.exit(1);
  }

  const programData = programLine.substring(programLine.indexOf(":") + 1).trim();
  if (!programData) {
    console.error("No instructions found after 'Program:'");
    process.exit(1);
  }

  const program = programData.split(",").map(Number);

  console.log("Registers:", { A, B, C });
  console.log("Program:", program);

  let outputs = [];
  let ip = 0;
  let registers = [A, B, C];

  while (ip < program.length) {
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
        break;
      case 6:
        registers[1] = Math.floor(registers[0] / (2 ** (operand <= 3 ? operand : registers[operand - 4])));
        break;
      case 7:
        registers[2] = Math.floor(registers[0] / (2 ** (operand <= 3 ? operand : registers[operand - 4])));
        break;
      default:
        console.error("Unknown opcode:", opcode);
        process.exit(1);
    }

    ip += 2;
  }

  const finalOutput = outputs.join(",");
  console.log("Final output:", finalOutput);
});
