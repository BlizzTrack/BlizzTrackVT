export default (handler: () => void) => {
    if (
        /complete|loaded|interactive/.test(document.readyState) && document.body
    ) {
        handler();
    } else {
        document.addEventListener("DOMContentLoaded", handler, false);
    }
};