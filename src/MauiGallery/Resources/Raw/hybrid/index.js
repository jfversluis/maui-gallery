// window.HybridWebView.InvokeDotNet calls a method on the object passed to SetInvokeJavaScriptTarget.
document.getElementById('greet').addEventListener('click', async () => {
  const name = document.getElementById('name').value;
  const greeting = await window.HybridWebView.InvokeDotNet('Greet', [name]);
  document.getElementById('greeting').textContent = greeting;
});

document.getElementById('device').addEventListener('click', async () => {
  const device = await window.HybridWebView.InvokeDotNet('GetDeviceInfo');
  const info = document.getElementById('info');
  info.replaceChildren();
  for (const [label, value] of [['Platform', device.platform], ['Version', device.version], ['Model', device.model], ['Idiom', device.idiom]]) {
    const dt = document.createElement('dt');
    dt.textContent = label;
    const dd = document.createElement('dd');
    dd.textContent = value;
    info.append(dt, dd);
  }
});
