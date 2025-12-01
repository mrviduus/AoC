// Sources: https://adventofcode.com/2024/day/1
const fs = require('fs');
const path = require('path');

const DECIMAL_BASE = 10;
const INPUT_FILE = path.join(__dirname, './../input.txt');

function readInputFile(filePath) {
  try {
    return fs.readFileSync(filePath, 'utf-8').split('\n');
  } catch (error) {
    console.error(`Error reading file from path ${filePath}:`, error);
    process.exit(1);
  }
}

function parseLine(line) {
  const parts = line.trim().split(/\s+/);
  if (parts.length === 2 && !isNaN(parts[0]) && !isNaN(parts[1])) {
    return [parseInt(parts[0], DECIMAL_BASE), parseInt(parts[1], DECIMAL_BASE)];
  }
  return null;
}

function calculateTotalDistance(left, right) {
  let totalDistance = 0;
  for (let i = 0; i < left.length; i++) {
    totalDistance += Math.abs(left[i] - right[i]);
  }
  return totalDistance;
}

// Main execution
const inputLines = readInputFile(INPUT_FILE);

const left = [];
const right = [];

inputLines.forEach(line => {
  const parsed = parseLine(line);
  if (parsed) {
    left.push(parsed[0]);
    right.push(parsed[1]);
  }
});

left.sort((a, b) => a - b);
right.sort((a, b) => a - b);

const totalDistance = calculateTotalDistance(left, right);

console.log(totalDistance);