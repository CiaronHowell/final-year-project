import React from 'react';
import '../css/nav.css';


class NavBarComponent extends React.Component {

    componentDidMount() {
        require('../scripts/navHandler'); 
    }

    render() {
        return (
            <nav id="nav" class="navStyle">
                <button id="saveButton" class="nav-button">Save</button>
                <button id="loadButton" class="nav-button">Load</button>
                <button id="newButton" class="nav-button">New</button>
            </nav>
        );
    }
}

export default NavBarComponent;