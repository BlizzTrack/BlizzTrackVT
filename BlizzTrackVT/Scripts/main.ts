import ready from "./ready";
import Sortable from 'sortablejs';

ready(() => {
    try {
        const s = Sortable.create(document.querySelector("#game-list"), {
            delayOnTouchOnly: true,
            delay: 150,
            dataIdAttr: "data-game",
            store: {
                /**
                 * Get the order of elements. Called once during initialization.
                 * @param   {Sortable}  sortable
                 * @returns {Array}
                 */
                get: (sortable: Sortable): string[] => {
                    var order = localStorage.getItem("game-order");
                    return order ? order.split('|') : [];
                },

                /**
                 * Save the order of elements. Called onEnd (when the item is dropped).
                 * @param {Sortable}  sortable
                 */
                set: (sortable: Sortable) => {
                    var order = sortable.toArray();
                    localStorage.setItem("game-order", order.join('|'));
                }
            },
            onUpdate: (ev: Sortable.SortableEvent) => {
                /*
                const items = document.querySelectorAll("[data-game]");

                let order: string[] = []
                items.forEach((item: HTMLElement) => {
                    order.push(item.dataset.game);
                });
                */
                console.log(s.toArray());
            }
        });

        s.sort(s.toArray());
    } catch (ex) {
        // nothing
    }
});