// Raw messages are plain strings. These samples send JSON: { "author": "...", "text": "..." }.
function show(message, fromWeb) {
  const log = document.getElementById('log');
  log.querySelector('p')?.remove();
  const bubble = document.createElement('div');
  bubble.className = fromWeb ? 'bubble web' : 'bubble';
  const author = document.createElement('span');
  author.className = 'author';
  author.textContent = message.author;
  bubble.append(author, message.text);
  log.append(bubble);
}

window.addEventListener('HybridWebViewMessageReceived', e => show(JSON.parse(e.detail.message), false));

document.getElementById('form').addEventListener('submit', e => {
  e.preventDefault();
  const message = { author: 'JavaScript', text: document.getElementById('text').value };
  window.HybridWebView.SendRawMessage(JSON.stringify(message));
  show(message, true);
});
