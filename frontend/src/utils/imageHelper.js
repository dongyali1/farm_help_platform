export const getImageUrl = (url) => {
  if (!url) return 'https://via.placeholder.com/200'
  if (url.startsWith('/uploads/')) {
    return `http://localhost:5000${url}`
  }
  return url
}