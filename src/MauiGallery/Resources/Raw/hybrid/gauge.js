// A global function, so .NET can call it with InvokeJavaScriptAsync("setLevel", ...).
function setLevel(level) {
  document.getElementById('gauge').style.setProperty('--level', level);
  document.getElementById('value').textContent = `${level}%`;
  const label = level < 34 ? 'Low' : level < 67 ? 'Medium' : 'High';
  document.getElementById('label').textContent = label;
  return label;
}

setLevel(0);
