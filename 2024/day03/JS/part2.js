const fs = require('fs');
const path = require('path');

const INPUT_FILE = path.join(__dirname, '../input.txt');

function readInputFile(filePath){
    try{
        return fs.readFileSync(filePath, 'utf8').split(`\n`);
    }catch(e){
        console.error(`error while reading file from path: ${filePath}`);
    }
}

function logicRegex() {
    return /mul\((\d+),(\d+)\)|do\(\)|don't\(\)/g;
  }
const enable = "do()";
const disable = "don't()";
    
let input = readInputFile(INPUT_FILE);

let sum = 0;
let enabled = true;
input.forEach(line => {
  const matches = line.matchAll(logicRegex());

  for (const match of matches) {
    const matchValue = match[0];
    switch (matchValue) {
      case enable:
        enabled = true;
        break;
      case disable:
        enabled = false;
        break;
      default:
        if (enabled) {
          const x = parseInt(match[1], 10);
          const y = parseInt(match[2], 10);
          sum += x * y;
        }
        break;
    }
  }
});

console.log(sum);