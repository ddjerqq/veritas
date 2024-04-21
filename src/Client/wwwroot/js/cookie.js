export function get(key) {
  const cookies = document.cookie.split(';');

  for (let i = 0; i < cookies.length; i++) {
    const cookie = cookies[i].split('=');
    if (cookie[0].trim() === key) {
      return cookie[1];
    }
  }

  return null;
}

export function set(name, value, expire) {
  console.log("setting", name, value);

  const date = new Date();
  date.setTime(date.getTime() + (expire * 24 * 60 * 60 * 1000));
  let expires = "expires=" + date.toUTCString();
  document.cookie = name + "=" + value + ";" + expires + ";path=/";
}